using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TriggerVolume : MonoBehaviour
{
    BoxCollider2D collider;

    public delegate void TriggerEntered(Actor coll);
    public TriggerEntered OnVolumeEntered;

    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.Log("This: " + gameObject.tag + " : " + gameObject.name + " , Hit: " + coll.gameObject.tag + " : " + coll.gameObject.name);
        Actor collActor = coll.gameObject.transform.parent.GetComponent<Actor>();
        if(null != OnVolumeEntered)
        {
            OnVolumeEntered(collActor);
        }
            
    }
}

