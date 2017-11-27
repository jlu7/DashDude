using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapperController : MonoBehaviour
{
    private static TapperController TC;

    public static TapperController GetInstance()
    {
        return TC;
    }

    public decimal MoneyAmount = 0;
    public Text VisibleMoneyAmount;
    public int currentTapAmount;

    protected Button TapArea;
    protected TapCharactersMenu TapCharactersMenu;
    protected List<ITapCharacters> TapCharacters;

    void Start()
    {
        TC = this;

        VisibleMoneyAmount = this.transform.Find("MoneyText/Amount").GetComponent<Text>();
        AddToVisibleMoneyAmount(0);
        currentTapAmount = 1;

        TapArea = transform.Find("TapArea").GetComponent<Button>();
        TapCharactersMenu = transform.Find("ScrollView").gameObject.AddComponent<TapCharactersMenu>();

        TapCharactersMenu.Initialize();
        TapArea.onClick.AddListener(() => AddToVisibleMoneyAmount(1));

        AddNewCharacterMenuButton("Character");
	}

    public void AddToVisibleMoneyAmount(decimal amount)
    {
        MoneyAmount += amount;
        VisibleMoneyAmount.text = MoneyAmount.ToString();
    }

    public void AddNewCharacterMenuButton(string character)
    {
        Transform Content = TapCharactersMenu.transform.Find("ViewPort/Content");
        GameObject button = Instantiate(Resources.Load<GameObject>("Assets/Characters/" + character)) as GameObject;

        button.transform.SetParent(Content);
        button.transform.localScale = new Vector3(1, 1, 1);
        button.transform.localPosition = new Vector3(0, 0, 0);
    }

    protected IEnumerator passiveLoop()
    {
        while (true)
        {
            foreach (ITapCharacters c in TapCharacters)
            {

            }
        }
    }
}
