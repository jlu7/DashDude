using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class PlayerController : Actor
{

    public int Jumps = 2;

    private IEnumerator COCharacterMovement;
    public float speed = 3f;
    public TriggerVolume triggerVolume;

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
        GetComponent<Rigidbody2D>().fixedAngle = true;
		PlayerStateChanged(PlayerState.NotDashing);
        triggerVolume.OnVolumeEntered += OnCollisionDetected;
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
            this.GetComponent<Rigidbody2D>().gravityScale = -this.GetComponent<Rigidbody2D>().gravityScale;
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
            this.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            GetComponent<Rigidbody2D>().gravityScale = 1;
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
            GetComponent<Rigidbody2D>().gravityScale = 0;
            this.GetComponent<Rigidbody2D>().AddForce(pos * 500);

            Jumps--;
        }
		
        //Debug.Log(transform.rotation);
    }

	void OnCollisionDetected(Actor collideActor)
	{
		//Debug.Log("OnCollisionDetected: " + reporter.GetID() + " , " + collider.GetID());
		if(collideActor.GetID() == "Enemy")
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
		if (coll.gameObject.tag == "Gem")
		{
		}
    }

}
