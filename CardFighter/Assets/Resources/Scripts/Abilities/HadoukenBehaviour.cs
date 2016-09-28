using UnityEngine;
using System.Collections;

public class HadoukenBehaviour : MonoBehaviour 
{
	// Update is called once per frame
	void Update () 
    {
        transform.position += new Vector3(.1f, 0, 0);
	}
}
