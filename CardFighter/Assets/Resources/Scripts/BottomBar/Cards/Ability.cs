using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ability : MonoBehaviour 
{
    public List<Action> Actions = new List<Action>();

    public virtual void Action()
    {
        for (int i = 0; i < Actions.Count; i++)
        {
            Actions[i].Do();
        }
    }
}
