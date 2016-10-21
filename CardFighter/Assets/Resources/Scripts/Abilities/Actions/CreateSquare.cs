using UnityEngine;
using System.Collections;

public class CreateSquare : Action
{
	public CreateSquare(GameObject owner) : base(owner)
	{
		Owner = owner;
	}

	public override void Do()
	{
		GameObject tmp = Object.Instantiate(Resources.Load<GameObject>("Prefabs/PlayerObjects/HitBox")) as GameObject;
		tmp.transform.parent = Owner.transform;
		tmp.GetComponent<HitBoxBehaviour>().Initialize(Owner, 1, 1f, new Vector3(0,0,0), new Vector3(4, 4, 0));
	}
}
