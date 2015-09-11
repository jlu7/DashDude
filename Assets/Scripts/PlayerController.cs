using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private IEnumerator COCharacterMovement;
    public float speed = 3f;

	void Awake ()
	{
	    COCharacterMovement = CharacterMovement();
	    StartCoroutine(COCharacterMovement);
	}

    IEnumerator CharacterMovement()
    {
        while (true)
        {
            //if (Input.GetMouseButtonDown(0))
            //{
     //           Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
       //         Debug.Log(pos);
         //       this.transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            //}
            yield return null; 
        }
    }

    private bool doOnce = false;
    private int count = 0;

    public void FixedUpdate()
    {
        Vector3 pos = new Vector3();
        if (count > 0)
        {
            count --;
        }

        if (doOnce && count == 0)
        {
            this.rigidbody2D.velocity = new Vector2(0,0);
            rigidbody2D.gravityScale = 1;
            doOnce = false;
        }

        if (Input.GetMouseButtonDown(0) && !doOnce)
        {
            count = 5;
            pos = Input.mousePosition;
            pos= Camera.main.ScreenToWorldPoint(pos);
            Debug.Log(pos);
            pos = pos - transform.position;
            doOnce = true;
            rigidbody2D.gravityScale = 0;
            this.rigidbody2D.AddForce(pos * 500);
        }
    }
}
