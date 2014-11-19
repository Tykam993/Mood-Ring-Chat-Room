using UnityEngine;
using System.Collections;

public class Synonym
{

    private Word _word;
    public Word Word
    {
        get
        {
            if (_word == null)
            {
                _word = new Word();
            }
            return _word;
        }
    }
    public bool IsDone
    {
        get
        {
            if (_word == null)
            {
                return false;
            }
            return _word.isDone;
        }
    }

    public Synonym(string word, System.Func<Word,Word> OnComplete, bool sendGetRequest=true)
    {
        _word = new Word();

        //This is the callback function--what will be called after the coroutine finishes
        //SynonymFinder.OnSynonymCompleteDelegate synDel = CallBack;
        if (sendGetRequest)
        {
            //this tells synonym finder to ping the server and get synonyms for the given word
            Utility.Instance.StartCoroutine(SynonymFinder.Instance.GetSynonymsForWord(word, _word, OnComplete));
        }    
    }


    public void PrintWhenReady()
    {
        Utility.Instance.StartCoroutine(PrintWhenReadyPrivate());
    }
    private IEnumerator PrintWhenReadyPrivate()
    {
        while (!IsDone)
        {
            yield return null;
        }

        Debug.Log(_word.ToString());
    }
}
