using UnityEngine;
using System.Collections;

public class StartUp : MonoBehaviour
{
    public GameObject ViewAnchorRef;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Initialize());
    }

    IEnumerator Initialize()
    {
        ViewController.GetInstance().Initialize(ViewAnchorRef.transform);
        yield return null;
    }
}
