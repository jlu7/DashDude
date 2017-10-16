using UnityEngine;
using System.Collections;
using System;

public class MoveStraight : Action
{
    public Vector3 TotalDisplacement = new Vector3(0, 0, 0);
    public float TimeTaken = 0f;

    public MoveStraight(GameObject owner, Vector3 totalDisplacement, float timeTaken) : base(owner)
	{
		Owner = owner;
        TotalDisplacement = totalDisplacement;
        TimeTaken = timeTaken;
    }

	public override void Do()
	{
        MoveStraight proxy = Owner.AddComponent<MoveStraight>();

        proxy.TotalDisplacement = this.TotalDisplacement;
        proxy.TimeTaken = this.TimeTaken;

        proxy.Owner = Owner;
        proxyAction = proxy;
        proxy.action();
	}
    
    public void action()
    {
        StartCoroutine(actionCO());
    }

    IEnumerator actionCO()
    {
        float rate = Vector3.Distance(Owner.transform.localPosition, Owner.transform.localPosition + TotalDisplacement) * TimeTaken;
        float t = 0;
        Vector3 startPos = Owner.transform.localPosition;

        while (t < 1.0)
        {
            Debug.Log(Owner.transform.localPosition);
            t += Time.deltaTime * rate;
            Owner.transform.localPosition = Vector3.Lerp(startPos, startPos + TotalDisplacement, Mathf.SmoothStep(0.0f, 1.0f, t));
            yield return null;
        }

        Complete = true;
        yield return null;
        Destroy(this);
    }
}
