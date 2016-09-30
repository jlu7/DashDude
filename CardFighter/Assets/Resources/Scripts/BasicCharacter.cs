using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BasicCharacter : MonoBehaviour 
{
    public int HP = 20;
    public Slider healthSlider;

    // if PlayerSide is true then the player is on the left side
    // otherwise it's the right

    public void Initialize(bool PlayerSide, GameObject Parent)
    {
        if (PlayerSide)
        {
            GameObject tmp = Instantiate(Resources.Load<GameObject>("Prefabs/BottomBar/HealthTick1")) as GameObject;
            tmp.transform.SetParent(Parent.transform, false);
            tmp.GetComponent<RectTransform>().sizeDelta = new Vector2(30 * 20, 70);
        }

        if (PlayerSide)
        {
            GameObject tmp = Instantiate(Resources.Load<GameObject>("Prefabs/BottomBar/HealthTick2")) as GameObject;
            tmp.transform.SetParent(Parent.transform, false);
            tmp.GetComponent<RectTransform>().sizeDelta = new Vector2(30 * 20, 70);
        }
    }
}
