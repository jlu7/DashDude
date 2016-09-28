using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{
    private static GameController GC;
    public GameObject UIControllerRef;

    GameObject Player1;
    GameObject Player2;

    public static GameController GetInstance()
    {
        if (GC == null)
        {
            GC = new GameObject("GameController").AddComponent<GameController>();
        }
        return GC;
    }

    public void Initialize()
    {
        UIControllerRef = Instantiate(Resources.Load<GameObject>("Prefabs/BottomBar/Camera")) as GameObject;

        // Create The Players
        Player1 = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerObjects/Player")) as GameObject;
        Player1.name = "Player1";
        Player1.transform.position = new Vector3(-5, 0, 0);
        Player1.AddComponent<Hadouken>();


        Player2 = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerObjects/Player")) as GameObject;
        Player2.name = "Player2";
        Player2.transform.position = new Vector3(5, 0, 0);
        UIController.GetInstance().Initialize(UIControllerRef.transform, Player1, Player2);
    }
}
