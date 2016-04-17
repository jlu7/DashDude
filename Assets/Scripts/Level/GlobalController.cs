using UnityEngine;
using System.Collections;

public class GlobalController : Singleton<GlobalController> 
{
	public int GemsPickedUp = 0;

	public void Initialize()
	{
		GemsPickedUp = PlayerPrefs.GetInt("GemsPickedUp");
	}

	public void AddGemsPickedUp(int amount)
	{
		GemsPickedUp += amount;
		PlayerPrefs.SetInt("GemsPickedUp", GemsPickedUp);
	}
}
