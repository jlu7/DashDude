using UnityEngine;
using System.Collections;

public class PlayerStatic : MonoBehaviour
{
    void Start()
    {
        PlayerController.GetInstance();
    }
}
