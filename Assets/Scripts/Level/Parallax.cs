using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour 
{
    public float ParallaxAmount;
    float prevPlayerPosition;

    public void Begin()
    {
        prevPlayerPosition = Levels.GetInstance().PC.gameObject.transform.position.x;
        StartCoroutine(StartParallax());
    }

    public IEnumerator StartParallax()
    {
        while(true)
        {
            Vector3 difPosition = new Vector3(this.gameObject.transform.position.x + (prevPlayerPosition - Levels.GetInstance().PC.gameObject.transform.position.x)*ParallaxAmount, this.gameObject.transform.position.y, this.gameObject.transform.position.z);

            prevPlayerPosition = Levels.GetInstance().PC.gameObject.transform.position.x;

            this.gameObject.transform.position = difPosition;
            yield return null;
        }
    }
}
