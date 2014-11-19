using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Word
{
    private string _word;
    public string WordString
    {
        get { return _word; }
        set { _word = value; }
    }
    private List<string> synonyms;
    public bool isDone;

    public Word()
    {
        _word = "";
        synonyms = new List<string>();
    }
    public Word(string w)
    {
        _word = w;
        synonyms = new List<string>();
    }
    public Word(string word, List<string> words)
    {
        _word = word;
        synonyms = words;
    }


    public List<string> GetSynonyms()
    {
        return synonyms;
    }

    public string GetWord()
    {
        return _word;
    }

    public void ClearSynonyms()
    {
        synonyms.Clear();
    }

    public void AddWords(List<string> words)
    {
        foreach (string word in words)
        {
            synonyms.Add(word);
        }
    }

    public void AddWord(string word)
    {
        if (!synonyms.Contains(word))
        {
            synonyms.Add(word);
        }
    }

    public override string ToString()
    {
        string result = "[";

        foreach (string s in synonyms)
        {
            result += s + ", ";
        }

        result = result.Substring(0, result.Length - 2);

        result += "]";

        return result;
    }


    public bool IsGarbage()
    {
        if (!isDone)
        {
            Debug.LogWarning("Warning! Asking if a word is garbage before it's finished contacting the server!");
        }

        return synonyms.Count == 0;
    }
}
