using UnityEngine;
using System.Collections;

public class HadoukenBehaviour : MonoBehaviour 
{
    public bool Owner;

    public HadoukenBehaviour(bool owner)
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
        if (col.gameObject.name == "Player2")
        {
            Debug.Log("BOOM");
            Destroy(this.gameObject);
        }
    }
}
