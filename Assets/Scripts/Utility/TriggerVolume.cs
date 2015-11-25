using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TriggerVolume : MonoBehaviour
{
    BoxCollider2D collider;

    public delegate void TriggerEntered(Collider2D coll);
    public TriggerEntered OnVolumeEntered;

    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        OnVolumeEntered(coll);
    }
}

