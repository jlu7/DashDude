using UnityEngine;
using System.Collections;

public abstract class Action
{
    public GameObject Owner;

    public Action(GameObject owner)
    {
        Owner = owner;
    }

    public abstract void Do();
}
