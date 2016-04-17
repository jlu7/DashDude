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

    public PlayerController PC { get; private set; }
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
		GlobalController.GetInstance().Initialize();
		camera.Initialize(CurrentLevel.Player.transform);
        PC = CurrentLevel.Player;
        level1.transform.Find("Images/BG Trees 1").GetComponent<Parallax>().Begin();
        level1.transform.Find("Images/BG Trees 2").GetComponent<Parallax>().Begin();
        PC.PlayerStateChange += (PlayerController.PlayerState ps) => CurrentPlayerState = ps;
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

