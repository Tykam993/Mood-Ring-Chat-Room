﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// The purpose of this class is to aggregate a Word -> Synonym dictionary over time for the user.
/// We're limited to 1000 GET requests for the synonym API, so we want to save as many requests as possible.
/// One way to do that is build up a local dictionary as the user chats.
/// </summary>W
[System.Serializable]
public class SynonymDictionary {
    public const float MIN_TIME_BETWEEN_SAVES = 10f;
    private static float _lastSave;
    public static float LastSave
    {
        get
        {
            if (_lastSave == null)
            {
                _lastSave = Time.time;
            }
            return _lastSave;
        }
    }

    public const string FILE_NAME = "synonym_dict.xml";

    [SerializeField]
    private List<string> _keys;
    [SerializeField]
    private List<ListWrapper> _values;


    private static SynonymDictionary _instance;
    public static SynonymDictionary SynInstance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SynonymDictionary();
            }
            return _instance;
        }
    }

    public SynonymDictionary()
    {
        _keys = new List<string>();
        _values = new List<ListWrapper>();
    }

    #region Saving/Loading hotkeys

    /// <summary>
    /// Loads saved hotkeys from XML. Returns whether it found existing file or not!
    /// </summary>
    public static bool LoadSynonymDictionary()
    {
        bool result = SaveMe.SaveMeInstance.Load();
        if (result)
        {
            SynInstance._keys = SaveMe.SaveMeInstance.keys;
            SynInstance._values = SaveMe.SaveMeInstance.values;
        }
        else
        {
            SynInstance._keys = new List<string>();
            SynInstance._values = new List<ListWrapper>();
        }

        return result;
    }


    public static void SaveSynonymDictionary()
    {
        SynInstance.Save();
        _lastSave = Time.time;
    }

    public static void ResetSynonymDictionary()
    {
        string fp = Path.Combine(Application.persistentDataPath, FILE_NAME);

        FileStream stream = new FileStream(fp, FileMode.Create);
        stream.Close();
    }
    
    /// <summary>
    /// Saves current hotkey setup to XML
    /// </summary>
    private void Save()
    {
        SaveMe.SaveMeInstance.Save(this);
    }

    #endregion

    #region Public Static Functions! Call these!

    public static void Add(string key, List<string> value)
    {
        SynInstance._Add(key, value);
    }

    public static string GetKey(int index)
    {
        return SynInstance._GetKey(index);
    }

    public static string DictionaryAsString()
    {
        return SynInstance.ToString();
    }

    public static bool Contains(string key)
    {
        return SynInstance._Contains(key);
    }

    public static bool TryGetValue(string key, out List<string> value)
    {
        return SynInstance._TryGetValue(key, out value);
    }

    #endregion

    #region Actual Dictionary Stuff
    //[SerializeField]
    //private MyDictionary _dictionary;
    //private MyDictionary SynDict
    //{
    //    get
    //    {
    //        if (_dictionary == null)
    //        {
    //            _dictionary = new MyDictionary();
    //        }
    //        return _dictionary;
    //    }
    //}
    //[System.Serializable]
    //class MyDictionary
    //{

        //public MyDictionary()
        //{
        //    _keys = new List<string>();
        //    _values = new List<List<string>>();
        //}

        private void _Add(string key, List<string> value)
        {
            ListWrapper wrappedValue = new ListWrapper(value);
            int indexOfExistentKey = _keys.IndexOf(key);

            // If the key already exists in our dictionary, replace the current value!
            if (indexOfExistentKey >= 0) {
                _values[indexOfExistentKey] = wrappedValue;
            }

                //else it's a new key and we can append it to the lists
            else {
            _keys.Add(key);
            _values.Add(wrappedValue);
            }
        }

        private bool _Contains(string key)
        {
            return _keys.Contains(key);
        }

        private bool _TryGetValue(string key, out List<string> value)
        {
            value = new List<string>();

            int keyIndex = _keys.IndexOf(key);

            if (keyIndex == -1) { return false; }
            else
            {
                value = _values[keyIndex].contents;
                return true;
            }
        }

        private string _GetKey(int index)
        {
            if (index < 0 || index >= _keys.Count)
            {
                throw new System.Exception("Index out of bounds-- GetKey");
            }
            return _keys[index];
        }

        public override string ToString()
        {
            string result = "";
            result +="MyDictionary printed----\n";
            result +="number of keys: " + _keys.Count.ToString() + "\n";
            result += "number of values: " + _values.Count.ToString() + "\n";
            for (int i = 0; i < _keys.Count; i++)
            {
                string printout = "-----Entry " + i + "-- [string, List<string>.Count]\n";
                string key = SynInstance._GetKey(i);
                List<string> value;
                SynInstance._TryGetValue(key, out value);
                printout += "[" + key + ", " + value.Count.ToString() + "]\n";

                result += printout;
            }

            return result;
        }
    //}

    


    private Dictionary<string, string[]> CreateDictionary(List<string> keys, List<string[]> values)
    {
        Dictionary<string, string[]> result = new Dictionary<string, string[]>();
        if (keys.Count != values.Count)
        {
            throw new System.Exception("Cannot create a new internal dictionary! There must be the same " +
            "number of keys as there are values.");
        }

        for (int i = 0; i < keys.Count; i++)
        {
            result.Add(keys[i], values[i]);
        }
        return result;
    }

    [System.Serializable]
    public class ListWrapper
    {
        [SerializeField]
        public List<string> contents;
        public ListWrapper() { contents = new List<string>(); }
        public ListWrapper(List<string> list) { contents = list; }


        public string this[int i]
        {
            get { return contents[i]; }
            set { contents[i] = value; }
        }
    }

    #endregion
    [System.Serializable]
    public class SaveMe
    {
        private static SaveMe _saveMe;
        public static SaveMe SaveMeInstance
        {
            get
            {
                if (_saveMe == null)
                {
                    _saveMe = new SaveMe();
                }
                return _saveMe;
            }
        }

        public List<string> keys;
        public List<ListWrapper> values;

        public SaveMe(List<string> k, List<ListWrapper> v)
        {
            keys = k;
            values = v;
        }

        public SaveMe()
        {
            keys = new List<string>();
            values = new List<ListWrapper>();
        }

        public void Save(SynonymDictionary sd)
        {
            keys = sd._keys;
            values = sd._values;

            string fp = Path.Combine(Application.persistentDataPath, FILE_NAME);

            XmlSerializer serializer = new XmlSerializer(typeof(SaveMe));
            using (FileStream stream = new FileStream(fp, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        public bool Load()
        {
            string fp = Path.Combine(Application.persistentDataPath, FILE_NAME);

            if (File.Exists(fp))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SaveMe));
                    using (FileStream stream = new FileStream(fp, FileMode.Open))
                    {
                        _saveMe = serializer.Deserialize(stream) as SaveMe;
                    }
                    return true;
                }
                catch (XmlException e)
                {
                    Debug.LogError("Error while loading Synonyms! StackTrace:");
                    Debug.Log(e.StackTrace);

                    _saveMe = new SaveMe();
                    return false;
                }

            }
            else
            {
                FileStream stream = new FileStream(fp, FileMode.Create);
                stream.Close();
                _saveMe = new SaveMe();
                return false;
            }
        }
    }

}
