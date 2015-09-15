using UnityEngine;
using System.Collections;

public class Actor : MonoBehaviour 
{
	public virtual string GetID()
	{
		return gameObject.tag;
	}
}
