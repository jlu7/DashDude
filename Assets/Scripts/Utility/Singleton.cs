using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    private static T _instance;
    public static T GetInstance()
    { 
        if (null == _instance)
        {
            GameObject single = new GameObject(typeof(T).ToString());
            _instance = single.AddComponent<T>();
        }
        return _instance;
    }
}

