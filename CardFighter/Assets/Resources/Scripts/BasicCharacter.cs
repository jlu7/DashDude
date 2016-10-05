using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BasicCharacter : MonoBehaviour 
{
    public int HP = 0;
    public RectTransform HealthSlider;

    // if PlayerSide is true then the player is on the left side
    // otherwise it's the right

    public void Initialize(GameObject Parent, bool PlayerSide, int hpAmount)
    {
        HP = hpAmount;

        if (PlayerSide)
        {
            GameObject tmp = Instantiate(Resources.Load<GameObject>("Prefabs/BottomBar/HealthTick1")) as GameObject;
            tmp.transform.SetParent(Parent.transform, false);
            Debug.Log(this.name + " HEALTH CREATED");
            HealthSlider = tmp.GetComponent<RectTransform>();
            HealthSlider.sizeDelta = new Vector2(30 * HP, 70);
        }

        if (!PlayerSide)
        {
            GameObject tmp = Instantiate(Resources.Load<GameObject>("Prefabs/BottomBar/HealthTick2")) as GameObject;
            tmp.transform.SetParent(Parent.transform, false);
            Debug.Log(this.name + " HEALTH CREATED");
            HealthSlider = tmp.GetComponent<RectTransform>();
            HealthSlider.sizeDelta = new Vector2(30 * HP, 70);
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        Debug.Log(this.name);
        HealthSlider.sizeDelta = new Vector2(30 * HP, 70);
    }
}
