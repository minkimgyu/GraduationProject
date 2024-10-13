using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Search for existing instance.
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null)
                {
                    // Need to create a new GameObject to attach the singleton to.
                    GameObject gameObject = new GameObject();
                    _instance = gameObject.AddComponent<T>();
                    gameObject.name = typeof(T).ToString() + " (Singleton)";
                    //DontDestroyOnLoad(gameObject);
                }
            }

            return _instance;
        }
    }
}