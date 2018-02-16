using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class PlayerController : Actor
{
    private const float MaxDistance = 3;

    public int Jumps = 2;

    private IEnumerator COCharacterMovement;
    public float speed = 4f;
    public TriggerVolume triggerVolume;

    private bool doOnce = false;
    private int count = 0;

    public enum PlayerState
    {
        Dashing,
        NotDashing,
        WallHug,
    }

    public PlayerState State = PlayerState.NotDashing;

	public delegate void PlayerStateChangeEvent(PlayerState pState);
	public PlayerStateChangeEvent PlayerStateChange;

    void Start()
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
		PlayerStateChanged(PlayerState.NotDashing);
        triggerVolume.OnVolumeEntered += OnCollisionDetected;
        StartCoroutine(MovementControls());
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

    private IEnumerator CurrentAction = null;
    bool WallHit = false;

    public IEnumerator MovementControls()
    {
        while (true)
        {
            Debug.Log(GetComponent<Rigidbody2D>().gravityScale);
            if (Input.GetKeyDown(KeyCode.A))
            {
                this.GetComponent<Rigidbody2D>().gravityScale = -this.GetComponent<Rigidbody2D>().gravityScale;
            }

            // End of Dash
            if (State == PlayerState.NotDashing)
            {
                GetComponent<Rigidbody2D>().gravityScale = 1;
                CurrentAction = null;
                WallHit = false;
            }

            // Check to see if player has clicked to dash
            if (Input.GetMouseButtonDown(0) && Jumps > 0)
            {
                PlayerStateChanged(PlayerState.Dashing);
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                pos = ConstrainClickDistance(pos);

                Jumps--;

                GetComponent<Rigidbody2D>().gravityScale = 0;

                CurrentAction = SimpleDash(new Vector3(pos.x, pos.y, this.transform.position.z));

                StartCoroutine(CurrentAction);
            }
            yield return null;
        }
    }

    private Vector3 ConstrainClickDistance(Vector3 clickTarget)
    {
        Vector3 finalPosition = new Vector3(0, 0, 0);

        finalPosition.z = this.transform.position.z;

        if ((clickTarget.x - transform.position.x) > MaxDistance)
        {
            finalPosition.x = transform.position.x + MaxDistance;
        }
        else if ((clickTarget.x - transform.position.x) < -MaxDistance)
        {
            finalPosition.x = transform.position.x - MaxDistance;
        }
        else
        {
            finalPosition.x = clickTarget.x;
        }

        if ((clickTarget.y - transform.position.y) > MaxDistance)
        {
            finalPosition.y = transform.position.y + MaxDistance;
        }
        else if ((clickTarget.y - transform.position.y) < -MaxDistance)
        {
            finalPosition.y = transform.position.y - MaxDistance;
        }
        else
        {
            finalPosition.y = clickTarget.y;
        }

        Debug.Log("Final Click Position: " + finalPosition);

        return finalPosition;
    }

    private IEnumerator SimpleDash(Vector3 position)
    {
        while (true)
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
            float step = 10 * Time.deltaTime;
            transform.position =
                Vector3.MoveTowards(
                    transform.position,
                    position, step);

            if (Vector3.Distance(transform.position, position) < 0.01f)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        //FREEZE

        yield return new WaitForSeconds(1f);

        State = PlayerState.NotDashing;
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
            WallHit = true;
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
