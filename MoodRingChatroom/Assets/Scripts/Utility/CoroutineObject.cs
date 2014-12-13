using UnityEngine;
using System.Collections;

public class CoroutineObject : MonoBehaviour {

    private static CoroutineObject _instance;
    public static CoroutineObject Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<CoroutineObject>();
                _instance.name = "Coroutine Object";
            }
            return _instance;
        }
    }
}
