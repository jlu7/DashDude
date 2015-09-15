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
        
        GameObject cam = GameObject.Find("Main Camera");
        SmoothCamera2D smoothCam2d = cam.GetComponent<SmoothCamera2D>();
        //PlayerController pc = PlayerController.GetInstance();
		Levels.GetInstance().Initialize(smoothCam2d);

        

    }
}
