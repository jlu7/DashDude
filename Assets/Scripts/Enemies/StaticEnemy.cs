using UnityEngine;
using System.Collections;

public class StaticEnemy : MonoBehaviour, IEnemy
{
    public void BeenHit()
    {
        PlayerController.GetInstance().Jumps = 2;
        Debug.Log("HIT");
        Destroy(this.gameObject);
    }

    public void HitPlayer()
    {
        Application.LoadLevel("main");        
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        /*Debug.Log("FUUU");

        if (coll.gameObject.tag == "Player" && PlayerController.GetInstance().State == PlayerController.PlayerState.Dashing)
        {
            BeenHit();
        }

        if (coll.gameObject.tag == "Player" && PlayerController.GetInstance().State == PlayerController.PlayerState.Dashing)
        {
            HitPlayer();
        }*/
    }
}
