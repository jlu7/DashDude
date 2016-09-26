using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppStartup : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        StartCoroutine(Startup());
    }

    IEnumerator Startup()
    {
        GameController.GetInstance().Initialize();
        yield return null;
    }
}
