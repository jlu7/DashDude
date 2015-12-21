using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class PlayerController : Actor
{

    //private static PlayerController PC;

    /*public static PlayerController GetInstance()
    {
        if (PC == null)
        {
            PC = ((GameObject)Instantiate((Resources.Load("Player/Player") as GameObject))).GetComponent<PlayerController>();
        }
        return PC;
    }*/

    public int Jumps = 2;

    private IEnumerator COCharacterMovement;
    public float speed = 3f;

    private bool doOnce = false;
    private int count = 0;

    public enum PlayerState
    {
        Dashing,
        NotDashing
    }

    public PlayerState State = PlayerState.NotDashing;

	public delegate void PlayerStateChangeEvent(PlayerState pState);
	public PlayerStateChangeEvent PlayerStateChange;

    void Start()
    {
        rigidbody2D.fixedAngle = true;
		PlayerStateChanged(PlayerState.NotDashing);
    }

	void OnDestroy()
	{
		//Levels.GetInstance().ActorCollision -= OnCollisionDetected;
	}

	void PlayerStateChanged(PlayerState pState)
	{
		State = pState;
		if(null != PlayerStateChange) PlayerStateChange(pState);
	}

    public void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            this.rigidbody2D.gravityScale = -this.rigidbody2D.gravityScale;
        }

        Vector3 pos = new Vector3();

        // Counts Dash Frames
        if (count > 0)
        {
            count--;
        } 
        
        // End of Dash
        if (doOnce && count == 0)
        {
            //State = PlayerState.NotDashing;
			PlayerStateChanged(PlayerState.NotDashing);
            this.rigidbody2D.velocity = new Vector2(0,0);
            rigidbody2D.gravityScale = 1;
            doOnce = false;
        }

        // Check to see if player has clicked to dash
        if (Input.GetMouseButtonDown(0) && !doOnce && Jumps > 0)
        {
			PlayerStateChanged(PlayerState.Dashing);
            count = 5;
            pos = Input.mousePosition;
            pos= Camera.main.ScreenToWorldPoint(pos);
            //Debug.Log(pos);
            pos = pos - transform.position;
            doOnce = true;
            rigidbody2D.gravityScale = 0;
            this.rigidbody2D.AddForce(pos * 500);

            Jumps--;
        }
        //Debug.Log(transform.rotation);
    }

	void OnCollisionDetected(Actor reporter, Actor collider)
	{
		Debug.Log("OnCollisionDetected: " + reporter.GetID() + " , " + collider.GetID());
		if(collider.GetID() == this.GetID())
		{
			if(reporter.GetID() == "Enemy")
			{
				if(State == PlayerState.Dashing)
				{
					Jumps = 2;
				}
				else if(State == PlayerState.NotDashing)
				{
					Levels.GetInstance().RestartLevel();
				}

			}
		}
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Wall")
        {
            Jumps = 2;
        }

        if (coll.gameObject.tag == "Death")
        {
			Levels.GetInstance().RestartLevel();
			Destroy(this);
        }

        if (coll.gameObject.tag == "Enemy" && State == PlayerState.Dashing)
        {
        }

        if (coll.gameObject.tag == "Player" && State == PlayerState.Dashing)
        {
        }
    }

}
