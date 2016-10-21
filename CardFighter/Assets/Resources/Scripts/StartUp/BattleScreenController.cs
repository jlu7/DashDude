using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

public class BattleScreenController : MonoBehaviour 
{
    private static BattleScreenController BS;
    public static BottomBar BottomBarRef;

    BasicCharacter Player1;
    BasicCharacter Player2;

    public static BattleScreenController GetInstance()
    {
        if (BS == null)
        {
            BS = new GameObject("UIController").AddComponent<BattleScreenController>();
        }
        return BS;
    }

    public void Initialize(Transform anchorRef)
    {
        // Create The Players
        GameObject tmpPlayer1 = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerObjects/Player")) as GameObject;
        tmpPlayer1.name = "Player1";
        tmpPlayer1.transform.position = new Vector3(-5, 0, 0);
        //tmpPlayer1.AddComponent<Hadouken>();

        GameObject tmpPlayer2 = Instantiate(Resources.Load<GameObject>("Prefabs/PlayerObjects/Player")) as GameObject;
        tmpPlayer2.name = "Player2";
        tmpPlayer2.transform.position = new Vector3(5, 0, 0);

        // Create the UI
        GameObject BottomBarGO = Instantiate(Resources.Load<GameObject>("Prefabs/BottomBar/BottomBar")) as GameObject;
        BottomBarGO.transform.SetParent(anchorRef, false);
        BottomBarRef = BottomBarGO.GetComponent<BottomBar>();

        List<Ability> abilities = new List<Ability>();
       
		List<Action> HadoukenActions = new List<Action>(){new Blast(tmpPlayer1)};
		Ability Hadouken = new Ability("Hadouken");
        Hadouken.Actions = HadoukenActions;
        abilities.Add(Hadouken);

		List<Action> bigMidHitActions = new List<Action>(){new CreateSquare(tmpPlayer1)};
		Ability bigMidHit = new Ability("Slash");
		bigMidHit.Actions = bigMidHitActions;
		abilities.Add(bigMidHit);

        BottomBarRef.Initialize(tmpPlayer1, abilities);
        GameObject TopBarGO = Instantiate(Resources.Load<GameObject>("Prefabs/BottomBar/TopBar")) as GameObject;
        TopBarGO.transform.SetParent(anchorRef, false);

        // Finally initialize the Players
        Player1 = tmpPlayer1.AddComponent<BasicCharacter>();
        Player1.Initialize(TopBarGO, true, 20, abilities);

        Player2 = tmpPlayer2.AddComponent<BasicCharacter>();
        Player2.Initialize(TopBarGO, false, 20, new List<Ability>());
    }

    public void playerLoseHealth(GameObject Player, int amount)
    {
        Player.GetComponent<BasicCharacter>().TakeDamage(amount);
    }
}

