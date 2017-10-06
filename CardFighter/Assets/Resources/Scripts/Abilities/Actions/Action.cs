using UnityEngine;
using System.Collections;

public abstract class Action : MonoBehaviour
{
    public GameObject Owner;
    public bool Complete = false;
    public Action proxyAction;

    public Action(GameObject owner)
    {
        Owner = owner;
    }

    public abstract void Do();
}
