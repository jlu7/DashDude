using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BottomBar : MonoBehaviour 
{
    private GameObject Player1Ref;
    public void Initialize(GameObject player1Ref, List<Ability> equippedAbilities)
    {
        Player1Ref = player1Ref;
        GameObject tmp = Instantiate(Resources.Load<GameObject>("Prefabs/BottomBar/Cards/Hadouken")) as GameObject;
        tmp.transform.SetParent(this.transform);
        //tmp.transform.parent = this.transform;

        tmp.transform.localScale = new Vector3(1, 1, 1);
        tmp.transform.localPosition = new Vector3(0, 0, 0);
        foreach (Ability ability in equippedAbilities)
        {
            tmp.transform.Find("Button").GetComponent<Button>().onClick.AddListener(
                () => 
                    (Player1Ref.GetComponent(ability.GetType()) as Ability).Action()
                );
        }
    }
}
