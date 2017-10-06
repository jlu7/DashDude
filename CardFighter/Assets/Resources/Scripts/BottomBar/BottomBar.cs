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

        int count = 0;

		foreach(Ability ability in equippedAbilities)
		{
			GameObject tmp = Instantiate(Resources.Load<GameObject>("Prefabs/BottomBar/Cards/" + ability.CardText)) as GameObject;
			tmp.transform.SetParent(this.transform);
			//tmp.transform.parent = this.transform;

			tmp.transform.localScale = new Vector3(1, 1, 1);
			tmp.transform.localPosition = new Vector3(CardDealAlgorithm(count, 275 * 2), 0, 0);

			tmp.transform.Find("Button").GetComponent<Button>().onClick.AddListener(
				() => 
				StartCoroutine(ability.Action())
			);
            count++;
		}
    }

    public float CardDealAlgorithm(int count, float oscillation)
    {

        float final = 0;

        int leftOrRight = 1;

        final = count / 2 * oscillation;

        if (count % 2 == 0)
        {
            leftOrRight = -1;
        }

        final = leftOrRight * final + (oscillation / 2 * leftOrRight);

        //float final = oscillation * (count / 2) * leftOrRight;
        return final;

    }
}
