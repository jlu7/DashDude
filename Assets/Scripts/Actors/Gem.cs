using UnityEngine;
using System.Collections;

public class Gem : Actor
{
	public GameObject CollideVolume;
	
	TriggerVolume triggerVolume;
	
	Rigidbody2D rb2d;
	
	void Start()
	{
		rb2d = GetComponent<Rigidbody2D>();
		//rb2d.
		if (null != CollideVolume)
		{
			triggerVolume = CollideVolume.GetComponent<TriggerVolume>();
			triggerVolume.OnVolumeEntered += TriggerEnter;
		}
	}

	void TriggerEnter(Actor colliderActor)
	{
		Debug.Log(colliderActor + " " + this.name);
		//Actor colliderActor = coll.gameObject.GetComponent<TriggerVolume>().transform.parent.GetComponent<Actor>();
		
		if (colliderActor)
		{
			Debug.Log("Collider is actor");
			if (colliderActor.GetID() == "Player")
			{
				GlobalController.GetInstance().AddGemsPickedUp(1);
				Debug.Log(GlobalController.GetInstance().GemsPickedUp);
				Destroy(this.gameObject);
			}
		}
	}
}
