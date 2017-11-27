using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter1 : ITapCharacters
{
    public decimal revenue = 0;
    public decimal currentCost = 100;
    public bool active = false;

    public IEnumerator PassiveLoop()
    {
        while (true)
        {
            TapperController.GetInstance().AddToVisibleMoneyAmount(revenue);
            yield return new WaitForSeconds(3f);
        }
    }

    public void BuyButtonAction()
    {
        if (TapperController.GetInstance().MoneyAmount >= currentCost)
        {
            TapperController.GetInstance().AddToVisibleMoneyAmount(-currentCost);
            currentCost += 50;
            revenue += 1;
        }
    }
}
