using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugErrors : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var audioListeners = Resources.FindObjectsOfTypeAll<AudioListener>();
        if (audioListeners.Length > 1)
        {
            Debug.LogWarning("PROBLEM: There are " + audioListeners.Length + " audio listeners.");
            foreach(var listener in audioListeners)
            {
                Debug.LogWarning(" > LISTENER:"+ GetGameObjectPath(listener.gameObject));
            }
        }
    }
    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }
}
