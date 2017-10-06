using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability : MonoBehaviour 
{
    public List<Action> Actions = new List<Action>();

	public string CardText = "TEST";

	public Ability(string cardText)
	{
		CardText = cardText;
	}

    public virtual IEnumerator Action()
    {
        for (int i = 0; i < Actions.Count; i++)
        {
            Actions[i].Do();

            while (!Actions[i].proxyAction.Complete)
            {
                yield return null;
            }
            Debug.Log("Completed: " + i);
        }
    }
}
