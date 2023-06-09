using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    public List<GameObject> hearts;

    private int health;

    private bool isFadingIn;

    private GameObject incomingHeart;

    void Start()
    {
        AudioManager.instance.Bar += MakeIncomingHeartToBlink;
    }

    public void SetHealth(int points)
    {
        health = points;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i=0; i<hearts.Count; i++) {
            hearts[i].SetActive(i <= health);
            hearts[i].GetComponent<CanvasRenderer>().SetAlpha(1f);
        }

        if (health < hearts.Count-1) {
            incomingHeart = hearts[health+1];
            incomingHeart.SetActive(true);
            incomingHeart.GetComponent<CanvasRenderer>().SetAlpha(0.8f);
        } else {
            incomingHeart = null;
        }

        if (isFadingIn == false) {
            StopCoroutine(Blink());
        }
    }

    void MakeIncomingHeartToBlink()
    {
        if (incomingHeart != null) {
            StartCoroutine(Blink());
        }
    }

    IEnumerator Blink()
    {
        isFadingIn = true;
        CanvasRenderer canvasRenderer = incomingHeart.GetComponent<CanvasRenderer>();

        for (float a=0f; a<0.8f; a+=0.05f){
            canvasRenderer.SetAlpha(a);
            yield return null;
        }

        isFadingIn = false;
    }
}
