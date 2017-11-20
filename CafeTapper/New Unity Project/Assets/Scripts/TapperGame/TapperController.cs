using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapperController : MonoBehaviour
{
    public decimal MoneyAmount = 0;
    public Text VisibleMoneyAmount;
    public int currentTapAmount;

    protected Button TapArea;

	// Use this for initialization
	void Start ()
    {
        VisibleMoneyAmount = this.transform.Find("MoneyText/Amount").GetComponent<Text>();
        AddToVisibleMoneyAmount(0);
        currentTapAmount = 1;

        TapArea = transform.Find("TapArea").GetComponent<Button>();

        TapArea.onClick.AddListener( () => AddToVisibleMoneyAmount(1));
	}

    public void AddToVisibleMoneyAmount(decimal amount)
    {
        MoneyAmount += amount;
        VisibleMoneyAmount.text = MoneyAmount.ToString();
    }
}
