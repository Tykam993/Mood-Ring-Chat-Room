#define DEBUG

using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;



public class SynonymFinder : MonoBehaviour
{

    public delegate void OnSynonymCompleteDelegate(Synonym synonym);

    /*
     * Big Huge Thesaurus info:
     * 
     * API key: 24a53241dfed7024109f310e4f552a4e
     * Version: 2
     * Generic GET request: http://words.bighugelabs.com/api/{version}/{api key}/{word}/{format}
     * 
     * How to make requests (with the specific API key):
     * 
     * Plain text
     *   http://words.bighugelabs.com/api/2/24a53241dfed7024109f310e4f552a4e/word/
     *   The trailing slash is required.
     *   
     *   XML
     *   http://words.bighugelabs.com/api/2/24a53241dfed7024109f310e4f552a4e/word/xml
     *   
     *   JSON
     *   http://words.bighugelabs.com/api/2/24a53241dfed7024109f310e4f552a4e/word/json
     *   
     *   Serialized PHP
     *   http://words.bighugelabs.com/api/2/24a53241dfed7024109f310e4f552a4e/word/php
     *   
     * */


    private static SynonymFinder _instance;
    public static SynonymFinder Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<SynonymFinder>();
                _instance.gameObject.name = "Synonym Finder Object";
            }

            return _instance;
        }
    }

    public enum TextFormat { PlainText, XML, JSON, SerializedPHP };

    /// <summary>
    /// Returns the appropriate GET request string for getting the synonym for a word.
    /// If no format is specified, the default format returned is JSON.
    /// </summary>
    /// <param name="word">The word to get synonyms for</param>
    /// <param name="format">Format to get the string back in</param>
    /// <returns>the URL request string</returns>
    private string GetRequestString(string word, TextFormat format = TextFormat.JSON)
    {
        string result = "http://words.bighugelabs.com/api/2/24a53241dfed7024109f310e4f552a4e/";
        result += word + "/";

        switch (format)
        {
            case TextFormat.JSON:
                result += "json";
                break;
            case TextFormat.PlainText:
                result += "";//don't add anything
                break;
            case TextFormat.SerializedPHP:
                result += "php";
                break;
            case TextFormat.XML:
                result += "xml";
                break;
        }

        return result;
    }


    public IEnumerator GetSynonymsForWord(string word, Word result, System.Func<Word,Word> OnComplete/*OnSynonymCompleteDelegate onSynonymComplete*/)
    {
        //Word result = new Word();
        result.isDone = false;
        string requestStr = GetRequestString(word);

        WWW returnedQuery = GET(requestStr);

#if DEBUG
        int count = 0;
#endif
        while (!returnedQuery.isDone)
        {
            yield return null;
#if DEBUG
            if (count > 300)
            {
                Debug.LogError("GET request timed out");
            }
            else
            {
                count++;
            }
#endif
        }//do nothing while it's processing

        string jsonString = returnedQuery.text; //for now we only support JSON

        var N = JSON.Parse(jsonString);

        //JSON parsing//
        JSONArray arr;
        string[] typesOfSpeech = { "verb", "adverb", "noun", "adjective" };
        foreach (string key in typesOfSpeech)
        {
            if (N[key] != null)
            {
                if (N[key]["syn"] != null)
                {
                    arr = N[key]["syn"].AsArray;
                    foreach (JSONNode str in arr)
                    {
                        //Only add the word if it doesn't contain a space
                        if (!str.Value.Contains(" "))
                        {
                            string sanString = Utility.SanitizeString(str.Value);
                            result.AddWord(sanString);
                        }
                    }
                }
            }
        }

        result.isDone = true;
        //return result;

        //run the delegate passed!
       // onSynonymComplete();
        OnComplete(result);
    }


    public WWW GET(string url)
    {
        WWW www = new WWW(url);
        Utility.Instance.StartCoroutine(WaitForRequest(www));
        return www;
    }

    public WWW POST(string url, Dictionary<string, string> post)
    {
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<string, string> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(url, form);

        Utility.Instance.StartCoroutine(WaitForRequest(www));
        return www;
    }

    private IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            //Debug.Log("WWW Ok!: " + www.text);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
}
