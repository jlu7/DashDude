using UnityEngine;
using System.Collections;

public class BouncyEnemyBrain : Actor {

    public GameObject MovementVolume;
    public GameObject CollideVolume;

    TriggerVolume triggerVolume;

    Rigidbody2D rb2d;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = .8f;
        //rb2d.
        if (null != CollideVolume)
        {
            triggerVolume = CollideVolume.GetComponent<TriggerVolume>();
            triggerVolume.OnVolumeEntered += (Collider2D coll) =>
                {
                    //Debug.Log("Hit: " + coll.gameObject.name);
                };
        }
    }

    void FixedUpdate()
    {
        if(null != MovementVolume)
        {
            //rb2d.AddForce(GetDirectionWithinBounds(MovementVolume.GetComponent<BoxCollider2D>()));
        }
        //rb2d.AddForce(new Vector2(1, 0));
        //Debug.Log("[ " + this.transform.position.x + " , " + this.transform.position.y + " ]");
    }

    Vector2 CurrentVector = new Vector2(1, 0);

    Vector2 GetDirectionWithinBounds(BoxCollider2D boxCol2d)
    {
        //Debug.Log(boxCol2d.bounds.extents.x + " , " + boxCol2d.bounds.extents.y);
        if (boxCol2d.bounds.extents.x + boxCol2d.transform.position.x < this.transform.position.x)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            CurrentVector = new Vector2(-1, 0);
        }
        else if (-boxCol2d.bounds.extents.x + boxCol2d.transform.position.x > this.transform.position.x)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            CurrentVector = new Vector2(1, 0);
        }

        //Debug.Log(CurrentVector);
        return CurrentVector;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        Actor colliderActor = coll.gameObject.GetComponent<PlayerController>();

        if (colliderActor)
        {
            Debug.Log("Collider is actor");
            if (colliderActor.GetID() == "Player" &&
               Levels.GetInstance().CurrentPlayerState == PlayerController.PlayerState.Dashing)
            {
                Destroy(this.transform.parent.gameObject);
            }
        }
    }
}
