using UnityEngine;
using System.Collections;

public class Hadouken : Ability
{
	public Hadouken(string cardText) : base(cardText)
	{
		CardText = cardText;
	}
	
    public override void Action()
    {
        GameObject tmp = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerObjects/HadoukenGO")) as GameObject;
        tmp.transform.parent = this.transform;
        tmp.GetComponent<HadoukenBehaviour>().Owner = true;
    }
}