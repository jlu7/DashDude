using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Levels : Singleton<Levels>
{
    public void Initialize()
    {
        GameObject level1Prefab = Resources.Load("Levels/Level1") as GameObject;
        Instantiate(level1Prefab);
    }
}

