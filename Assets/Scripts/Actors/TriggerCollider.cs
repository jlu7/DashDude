using UnityEngine;
using System;
using System.Collections;

public class TriggerCollider : MonoBehaviour 
{

	public delegate void CollisionEvent(GameObject Trigger, GameObject collider);
	public CollisionEvent Collided;

	// Use this for initialization
	void Start () 
	{
		if(!gameObject.GetComponent<Collider2D>())
		{
			gameObject.AddComponent<Collider2D>();
		}
	}

	void OnTriggerEnter2D(Collider2D coll)
	{
		Debug.Log("Collided With trigger");
		if(null != Collided) Collided(this.transform.parent.gameObject, coll.transform.parent.gameObject);
	}
}
