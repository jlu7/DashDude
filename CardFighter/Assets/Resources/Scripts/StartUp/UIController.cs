﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

public class UIController : MonoBehaviour 
{
    private static UIController VC;
    private static Stack<Transform> Views;
    private Transform AnchorRef;

    public static UIController GetInstance()
    {
        if (VC == null)
        {
            VC = new GameObject("ViewController").AddComponent<UIController>();
        }
        return VC;
    }

    public void Initialize(Transform anchorRef)
    {
        // Create The FrontPage
        Views = new Stack<Transform>();
        AnchorRef = anchorRef;
        Views.Push(AnchorRef);
        GameObject Game = Instantiate(Resources.Load<GameObject>("Prefabs/BottomBar/BottomBar")) as GameObject;
        Game.transform.SetParent(AnchorRef, false);

        Views.Push(Game.transform);
        //GameObject FrontPage = Instantiate(Resources.Load<GameObject>("CardDetail/CardDetail")) as GameObject;
    }

    public GameObject CreateView(string ViewLocation)
    {
        PushView(Instantiate(Resources.Load<GameObject>(ViewLocation)) as GameObject);
        return Views.Peek().gameObject;
    }

    public void PushView(GameObject NewView)
    {
        Destroy(Views.Pop().gameObject);
        NewView.transform.parent = AnchorRef;
        NewView.transform.localScale = new Vector3(1, 1, 1);
        NewView.transform.localPosition = new Vector3(0, 0, 0);

        Views.Push(NewView.transform);
    }
}

