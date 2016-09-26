using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppStartup : MonoBehaviour
{
    public GameObject UIControllerRef;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Startup());
    }

    IEnumerator Startup()
    {
        UIController.GetInstance().Initialize(UIControllerRef.transform);
        yield return null;
    }
}
