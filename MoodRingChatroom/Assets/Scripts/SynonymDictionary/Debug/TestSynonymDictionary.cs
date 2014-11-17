using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This MB tests the features of the SynonymDictionary
/// 
///     Features:
///         - Saving/Loading to XML file
///         - Adding to/reading from dictionary
/// </summary>
public partial class TestSynonymDictionary : SerializedSingleton<TestSynonymDictionary> {

    public bool runSynonymDictionaryTests = false;

	// Use this for initialization
	void Start () {
        if (!runSynonymDictionaryTests) { return; }
        //Debug.Log(Application.persistentDataPath);

        Debug.Log(SynonymDictionary.DictionaryAsString()); //prints out current dictionary

        Debug.Log("-----Loaded currently saved dictionary-----");
        if (SynonymDictionary.LoadSynonymDictionary())
        {
            Debug.Log("  ---Found a saved file---");
        }
        else { Debug.Log("  ---Created new save file---"); }
        Debug.Log(SynonymDictionary.DictionaryAsString());



        SynonymDictionary.Add("starcraft2 races", new List<string>() {
            "zerg", "terran", "protoss"});
        SynonymDictionary.Add("LoL Roles", new List<string>() {
            "ADC", "Mid", "Top", "Jungle", "Support"
        });
        Debug.Log("-----Added test string/List<string> to dictionary-----");
        Debug.Log(SynonymDictionary.DictionaryAsString()); //prints out current dictionary after adding tests


        SynonymDictionary.SaveSynonymDictionary();
        Debug.Log("-----Saved current dictionary-----");
        Debug.Log(SynonymDictionary.DictionaryAsString());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
