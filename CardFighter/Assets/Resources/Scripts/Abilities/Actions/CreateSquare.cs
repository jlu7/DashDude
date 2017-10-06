using UnityEngine;
using System.Collections;
using System;

public class CreateSquare : Action
{
	public CreateSquare(GameObject owner) : base(owner)
	{
		Owner = owner;
	}

	public override void Do()
	{
        CreateSquare dumb = Owner.AddComponent<CreateSquare>();
        dumb.Owner = Owner;
        proxyAction = dumb;
        dumb.blah();
	}
    
    public void blah()
    {
        StartCoroutine(blahCO());
    }

    IEnumerator blahCO()
    {
        GameObject tmp = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerObjects/HitBox")) as GameObject;
        tmp.transform.parent = Owner.transform;
        tmp.GetComponent<HitBoxBehaviour>().Initialize(Owner, 1, 1f, new Vector3(0, 0, 0), new Vector3(4, 4, 0));
        yield return new WaitForSeconds(5f);
        Complete = true;
        yield return null;
    }
}
