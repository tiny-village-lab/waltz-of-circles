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
        if (GameManager.instance.GameIsOver()) {
            hearts[0].SetActive(false);
            return;
        }

        for (int i=0; i<hearts.Count; i++) {
            hearts[i].SetActive(i <= health-1);
            hearts[i].GetComponent<CanvasRenderer>().SetAlpha(1f);
        }

        if (health < hearts.Count) {
            incomingHeart = hearts[health];
            incomingHeart.SetActive(true);
            incomingHeart.GetComponent<CanvasRenderer>().SetAlpha(0.2f);
        } else {
            incomingHeart = null;
        }

    }

    public int GetHealth()
    {
        return health;
    }
}
