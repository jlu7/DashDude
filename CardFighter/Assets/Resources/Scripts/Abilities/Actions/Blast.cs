using UnityEngine;
using System.Collections;

public class Blast : Action
{
    public Blast(GameObject owner) : base(owner)
    {
        Owner = owner;
    }

    public override void Do()
    {
        GameObject tmp = Object.Instantiate(Resources.Load<GameObject>("Prefabs/PlayerObjects/HadoukenGO")) as GameObject;
        tmp.transform.parent = Owner.transform;
        tmp.GetComponent<HadoukenBehaviour>().Initialize(Owner);
    }
}
