using UnityEngine;
using System.Collections;

public class StaticEnemyTrigger : Actor 
{

    void OnTriggerEnter2D(Collider2D coll)
    {
		Actor colliderActor = coll.GetComponent<Actor>();

		if(colliderActor)
		{
			Debug.Log("Collider is actor");
			Levels.GetInstance().ActorCollision(this, colliderActor);
			if(colliderActor.GetID() == "Player" && 
			   Levels.GetInstance().CurrentPlayerState == PlayerController.PlayerState.Dashing)
			{
				Destroy(this.transform.parent.gameObject);
			}
		}

        /*if (coll.gameObject.tag == "Player" && PlayerController.GetInstance().State == PlayerController.PlayerState.Dashing)
        {
            PlayerController.GetInstance().Jumps = 2;
            Destroy(this.transform.parent.gameObject);
        }

        if (coll.gameObject.tag == "Player" && PlayerController.GetInstance().State == PlayerController.PlayerState.NotDashing)
        {
            Application.LoadLevel("main");
        }*/
    }
}
