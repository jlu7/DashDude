using UnityEngine;
using System.Collections;

public class Bootstrap : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        InitializeGame();
	}

    void InitializeGame()
    {
        Levels.GetInstance().Initialize();
        GameObject cam = GameObject.Find("Main Camera");
        SmoothCamera2D smoothCam2d = cam.GetComponent<SmoothCamera2D>();
        PlayerController pc = PlayerController.GetInstance();

        smoothCam2d.Initialize(pc.gameObject.transform);

    }
}
