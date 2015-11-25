using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Levels : Singleton<Levels>
{
	protected bool levelRunning = false;
	protected LevelController CurrentLevel;
	protected SmoothCamera2D camera;

	public delegate void ActorCollisionEvent(Actor reporter, Actor collider);
	public ActorCollisionEvent ActorCollision;


    public PlayerController.PlayerState CurrentPlayerState { get; private set; }

    public void Initialize(SmoothCamera2D camera)
    {
		this.camera = camera;
        CurrentPlayerState = PlayerController.PlayerState.NotDashing;
		StartLevel();
    }

	public void StartLevel()
	{
		GameObject level1Prefab = Resources.Load("Levels/Level2") as GameObject;
		GameObject level1 = Instantiate(level1Prefab) as GameObject;
		CurrentLevel = level1.AddComponent<LevelController>();
		CurrentLevel.Initialize();
		camera.Initialize(CurrentLevel.Player.transform);
		PlayerController pc = CurrentLevel.Player;
		pc.PlayerStateChange += (PlayerController.PlayerState ps) => CurrentPlayerState = ps;
		levelRunning = true;
	}

	public void RestartLevel()
	{
		Debug.Log("Restart Level!");
		if(levelRunning)
		{
			levelRunning = false;
			GameObject.Destroy(CurrentLevel.gameObject);
			CurrentLevel = null;
			StartLevel();
		}
	}
}

