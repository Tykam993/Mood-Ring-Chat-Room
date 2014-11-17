using UnityEngine;
using System.Reflection;

/// TAKEN from becksebenius' bitbucket:
/// https://bitbucket.org/becksebenius/serialized-singletons/src/2eedde0051abc756856b03d97a4856f2a4c40862/Singleton/SerializedSingleton.cs?at=master

// Base class to make it easier to identify serialized singletons
public class SerializedSingletonBase : MonoBehaviour { }

public class SerializedSingleton<T> : SerializedSingletonBase
    where T : SerializedSingletonBase
{

    // Instance of the singleton
    static T instance;
    public static T Instance
    {
        get
        {
            // Is singleton registered?
            if (!instance)
            {
                // If not, do a brute force search for the instance
                instance = GameObject.FindObjectOfType(typeof(T)) as T;

                //If it can't be found, throw an error
                if (!instance)
                {
                    Debug.LogError("Could not find instance of singleton type " + typeof(T).Name + ".");
                    return null;
                }
            }
            return instance;
        }
    }

    public void Awake()
    {
        var type = typeof(T);

        // Make sure we're deriving properly (no way to do this type-safe unfortunately)
        if (GetType() != type)
        {
            Debug.LogError("Classes deriving from serialized singleton must use their own class name as the generic parameter.");
            return;
        }

        // Locate all static fields on the class
        var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var field in fields)
        {
            // Don't serialize if it's marked non-serialized
            if (field.IsNotSerialized) continue;

            // Find the generated instance field (will be marked private)
            var instanceFieldName = "_" + field.Name;
            var instanceField = type.GetField(instanceFieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            // Throw a warning if the field couldn't be found
            if (instanceField == null)
            {
                Debug.LogWarning("Field " + field.Name + " could not be initialized.", this);
                continue;
            }

            // Get the value from the instance field
            var value = instanceField.GetValue(this);

            // And set it onto the static field
            field.SetValue(null, value);
        }

        // Register the singleton
        instance = this as T;

        // OnAwake so that Singletons can still use the Awake function
        OnAwake();
    }

    public virtual void OnAwake() { }
}