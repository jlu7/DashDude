using System.Collections;
using UnityEngine.UI;

public interface ITapCharacters
{
    /// <summary>
    /// Each Tap Character is a passive bonus to money
    /// </summary>
    /// <returns></returns>
    IEnumerator PassiveLoop();

    /// <summary>
    /// Button Action for the purchase of a character
    /// </summary>
    void BuyButtonAction();
}
