using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class' main (and only public static function) takes in a string, representing
/// a word that we ostensibly want to add to our dictionary. This class decides
/// whether we ping the web server or if we can get the word from our local dictionary.
/// It's also responsible for creating Synonym objects and passing them along.
/// </summary>
public class ProcessWord {

    private static ProcessWord _instance;
    private static ProcessWord Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ProcessWord();
            }
            return Instance;
        }
    }

    public static void Process(string word)
    {
        //First throw the word out if it's too long!
        if (word.Length > MySQLDictionary.MAX_WORD_LENGTH) { return; }
        word = Utility.SanitizeString(word);

        ////Now determine if we already have the word!
        //List<string> synonyms;
        //if (SynonymDictionary.TryGetValue(word, out synonyms)) //if we find it in the dictionary
        //{
        //    Word wordWithSynonyms = new Word(word, synonyms);
        //
        //    //TO-DO: Now that we have the synonyms, send it to a controller that converts the strings into a list of EmotionIdeals
        //    CallMeToPassOnWord(wordWithSynonyms);
        //}
        if (MySQLDictionary.DatabaseContainsEntry(word))
        {
            MySQLDictionary.WordAndEmoIdeal result = MySQLDictionary.GetWordFromDatabase(word);

            //Now that we have the word and emotion ideal, we can pass add it to our model
            EmotionModel.ChangeStateByAddingEmotions(result.emoEnum);
        }
        else //else we need to ping the server!
        {
            //SynonymFinder.OnSynonymCompleteDelegate synDelegate = Instance.CallMeWhenDonePingingServer();

            /* This will create a get request for a new Word that will be populated with the word's
             * synonyms. The callback is what's called once it's finished getting the synonyms.
             * In this case, after getting the synonyms we want to
             *  - Add the new word/synonynms to the dictionary
             *  - Do something with the new Word object, to convert it into EmotionIdeals */
            System.Func<Word,Word> callback = CallMeWhenDonePingingServer;
            Synonym newWord = new Synonym(word, callback);
        }
    }

    /// <summary>
    /// Given a word, we want to add the word and the synonyms to the dictionary (given
    /// that it's a valid word) and save it. Then we want to pass the word off
    /// to the controller that will convert it into a list of EmotionIdeals.
    /// </summary>
    /// <param name="word">The word with its synonyms. Note this word could be garbage!</param>
    /// <returns></returns>
    public static Word CallMeWhenDonePingingServer(Word word)
    {
        string wordStr = word.GetWord();

        if (word.IsGarbage())
        {
            return word;
        }
        //TO-DO: Now that we have the filled word, we need to get the EmotionIdeal it represents!
        EmotionModel.EmotionIdeal emotionIdeal = Emotions.GetEmotionIdealAssociated(wordStr);

        //Add the new entry to the database, as long as it's not None.
        //And then have it affect the emotional model
        if (emotionIdeal != EmotionModel.EmotionIdeal.None)
        {
            MySQLDictionary.AddEntry(wordStr, emotionIdeal);
            EmotionModel.ChangeStateByAddingEmotions(emotionIdeal);
        }

        ////add the new word to the dictionary
        //SynonymDictionary.Add(word.GetWord(), word.GetSynonyms());
        //
        ////If we should save, do it
        //if (Time.time - SynonymDictionary.LastSave > SynonymDictionary.MIN_TIME_BETWEEN_SAVES)
        //{
        //    SynonymDictionary.SaveSynonymDictionary();
        //}

        return word;
    }
}
