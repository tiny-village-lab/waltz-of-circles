using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    public List<GameObject> hearts;

    private int health;

    private GameObject incomingHeart;

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
            incomingHeart.GetComponent<CanvasRenderer>().SetAlpha(0.4f);
        } else {
            incomingHeart = null;
        }
    }
}
