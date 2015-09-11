using UnityEngine;
using System.Collections;

public class StaticEnemyTrigger : MonoBehaviour 
{
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            PlayerController.GetInstance().Jumps = 2;
            Destroy(this.transform.parent.gameObject);
        }

        if (coll.gameObject.tag == "Player")
        {
            Debug.Log("BAM");
        }
    }
}
