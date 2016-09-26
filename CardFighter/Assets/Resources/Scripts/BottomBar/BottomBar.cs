using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BottomBar : MonoBehaviour 
{
    private GameObject Player1Ref;
    public void Initialize(GameObject player1Ref)
    {
        Player1Ref = player1Ref;
        GameObject tmp = Instantiate(Resources.Load<GameObject>("Prefabs/BottomBar/Cards/Hadouken")) as GameObject;
        tmp.transform.parent = this.transform;
        tmp.transform.localScale = new Vector3(1, 1, 1);
        tmp.transform.localPosition = new Vector3(0, 0, 0);
        Player1Ref.AddComponent<Hadouken>();
        tmp.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => Player1Ref.GetComponent<Hadouken>().Action());
    }
}
