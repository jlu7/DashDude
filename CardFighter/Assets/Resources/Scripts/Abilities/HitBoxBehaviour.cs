using UnityEngine;
using System.Collections;

public class HitBoxBehaviour : MonoBehaviour 
{
	public bool Owner;
	public int Durability = 0;
	public float Life = 0f;
	public Vector3 Position = new Vector3 (0, 0, 0);
	public Vector3 Scale = new Vector3 (0, 0, 0);

	public void Initialize(bool owner, int durability, float life, Vector3 position, Vector3 scale)
	{
		Owner = owner;
		Durability = durability;
		Life = life;
		Position = position;
		Scale = scale;
        this.transform.localScale = Scale;
        this.transform.localPosition = Position;
	}

	// Update is called once per frame
	void Update () 
	{
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		Debug.Log(col.gameObject.name);

		if (Owner)
		{
			if (col.gameObject.name == "Player2")
			{
				BattleScreenController.GetInstance().playerLoseHealth(col.gameObject, 3);
				Debug.Log("BOOM");
				Destroy(this.gameObject);
			}
		}
		else
		{
			if (col.gameObject.name == "Player1")
			{
				BattleScreenController.GetInstance().playerLoseHealth(col.gameObject, 3);
				Debug.Log("BOOM");
				Destroy(this.gameObject);
			}
		}
	}
}
