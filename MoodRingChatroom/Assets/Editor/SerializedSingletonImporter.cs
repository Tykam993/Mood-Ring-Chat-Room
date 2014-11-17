using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Reflection;
using System;
using System.Collections.Generic;

/// <summary>
/// TAKEN FROM becksebenius' bitbucket:
/// https://bitbucket.org/becksebenius/serialized-singletons/src/2eedde0051abc756856b03d97a4856f2a4c40862/Singleton/Editor/SerializedSingletonImporter.cs?at=master
/// </summary>

[InitializeOnLoad]
public class SerializedSingletonImporter : AssetPostprocessor
{
    static SerializedSingletonImporter()
    {
        // Get list of scripts which were imported on the last reload
        var str = EditorPrefs.GetString("_Singleton_Imports", "");

        // Reset the script reload list
        EditorPrefs.SetString("_Singleton_Imports", "");

        // Parse the scripts after finished initializing engine
        EditorApplication.delayCall += () =>
        {

            bool doRefresh = false;

            // Split the script paths out (separated by commas)
            var paths = str.Split(',');

            foreach (var path in paths)
            {
                if (path == "")
                {
                    continue;
                }
                doRefresh |= TestScript(path);
            }

            //Refresh the asset database if any of the scripts were changed
            if (doRefresh)
            {
                AssetDatabase.Refresh();
            }

        };
    }

    static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
    {
        // Collect all script files which were just imported
        List<string> paths = new List<string>();
        foreach (var path in importedAssets)
        {
            var file = new FileInfo(path);
            if (file.Extension != ".cs")
            {
                // not a script file
                continue;
            }

            paths.Add(path);
        }

        // Serialize to csv
        var str = "";
        foreach (var path in paths)
        {
            str += path + ",";
        }

        // Save in editor prefs, since we're facing an assembly reload after importing scripts
        EditorPrefs.SetString("_Singleton_Imports", str);
    }

    // Used for script generation
    delegate void AddLineDelegate(params object[] text);

    // Test a given script path for being a singleton
    // Returns whether the script was indeed a singleton and was changed
    public static bool TestScript(string path)
    {
        var file = new FileInfo(path);

        // Find the monoscript associated with this script
        var monoscript = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)) as MonoScript;
        if (monoscript == null)
        {
            // Not an indexed script, can't be the singleton root class
            return false;
        }

        var classType = monoscript.GetClass();
        if (classType == null)
        {
            //Not supported by unity (probably a generic)
            return false;
        }

        if (!typeof(SerializedSingletonBase).IsAssignableFrom(classType))
        {
            // isn't a singleton
            return false;
        }

        List<string> namespaces = new List<string>();
        using (StreamReader sr = new StreamReader(path))
        {
            string line;
            bool commentedOut;
            while ((line = sr.ReadLine()) != null)
            {
                int usingInd = line.LastIndexOf("using");
                if (usingInd != -1)
                {
                    int semicolonInd = line.LastIndexOf(';');
                    if (semicolonInd == -1) continue;

                    int start = usingInd + 6;
                    int end = semicolonInd - start;
                    namespaces.Add(line.Substring(usingInd + 6, end));
                }
            }
        }
        if (!namespaces.Contains("System")) namespaces.Add("System");

        // Generate filename for _Instance class
        file = new FileInfo(
            path.Substring(0, path.Length - file.Extension.Length)
            + "_Instance"
            + file.Extension
        );

        // Get all static fields on the singleton
        var fields = classType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        // Build the _Instance class
        var builder = new StringBuilder();
        int tab = 0;

        AddLineDelegate addLine = (text) =>
        {
            for (int i = 0; i < tab; i++) builder.Append("\t");
            foreach (var s in text) builder.Append(s.ToString());
            builder.Append("\n");
        };

        foreach (var n in namespaces)
        {
            addLine("using " + n + ";");
        }
        addLine();
        addLine("public partial class ", classType.Name);
        addLine("{");
        addLine();
        tab++;

        foreach (var field in fields)
        {
            if (field.IsNotSerialized) continue;
            addLine("[SerializeField]");
            addLine(field.FieldType.Name, " _", field.Name, ";");
            addLine();
        }

        tab--;
        addLine("}");

        // Compile builder
        var fileData = builder.ToString();

        // Nothing has changed, no need to recompile!
        if (File.Exists(file.FullName) && File.ReadAllText(file.FullName) == fileData)
        {
            return false;
        }

        // Write out to the file
        File.WriteAllText(file.FullName, builder.ToString());

        // And return that something has changed
        return true;
    }
}