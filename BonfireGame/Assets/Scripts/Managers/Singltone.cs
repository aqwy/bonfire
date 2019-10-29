using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singltone<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _Instance;

    public static T Instance
    {
        get
        {
            if (!_Instance)
            {
                _Instance = FindObjectOfType<T>();
            }

            if (!_Instance)
            {
                Debug.Log("Cant find " + typeof(T));
            }
            return _Instance;
        }
    }
}
