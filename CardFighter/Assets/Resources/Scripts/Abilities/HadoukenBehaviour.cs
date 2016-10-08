using UnityEngine;
using System.Collections;

public class HadoukenBehaviour : MonoBehaviour 
{
    public bool Owner;

    public void Initialize(bool owner)
    {
        Owner = owner;
    }

	// Update is called once per frame
	void Update () 
    {
        transform.position += new Vector3(.1f, 0, 0);
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
