using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameController : MonoBehaviour 
{
    private static GameController GC;
    public GameObject Camera;
    private static Stack<Transform> Views;
    private Transform AnchorRef;

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
        Camera = Instantiate(Resources.Load<GameObject>("Prefabs/BottomBar/Camera")) as GameObject;

        // Create The FrontPage
        Views = new Stack<Transform>();

        BattleScreenController.GetInstance().Initialize(Camera.transform);
    }


    public GameObject CreateView(string ViewLocation)
    {
        PushView(Instantiate(Resources.Load<GameObject>(ViewLocation)) as GameObject);
        return Views.Peek().gameObject;
    }

    public void PushView(GameObject NewView)
    {
        Destroy(Views.Pop().gameObject);
        NewView.transform.parent = AnchorRef;
        NewView.transform.localScale = new Vector3(1, 1, 1);
        NewView.transform.localPosition = new Vector3(0, 0, 0);

        Views.Push(NewView.transform);
    }
}
