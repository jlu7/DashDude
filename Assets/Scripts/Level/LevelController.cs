using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour 
{

	GameObject LevelStart;
	GameObject LevelEnd;
	public PlayerController Player {get; private set;}

	public void Initialize()
	{
		Transform stateChangeVolumes = transform.Find("StateChangeVolumes");

		LevelStart = stateChangeVolumes.Find("LevelStart").gameObject;
		LevelEnd = stateChangeVolumes.Find("LevelEnd").gameObject;
		InitializePlayer();
		InitializeEndState();
	}

	void InitializePlayer()
	{
		Player = (Instantiate(Resources.Load("Player/Player")) as GameObject).GetComponent<PlayerController>();
		Player.transform.position = LevelStart.transform.position;
		Player.transform.SetParent(this.transform);
		//player.GetComponent<PlayerController>().Initialize();
	}

	void InitializeEndState()
	{
		LevelEnd.AddComponent<TriggerCollider>().Collided += CheckEnterEndState;
	}

	void CheckEnterEndState(GameObject trigger, GameObject collider)
	{
		Actor colliderActor = collider.GetComponent<Actor>();
		if(colliderActor && colliderActor.GetID() == "Player")
		{
			Debug.Log("Level Completed!");
			Levels.GetInstance().RestartLevel();
		}
	}

}
