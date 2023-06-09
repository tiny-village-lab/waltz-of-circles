using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    public List<GameObject> hearts;

    private int health;

    public void SetHealth(int points)
    {
        health = points;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i=0; i<hearts.Count; i++) {
            hearts[i].SetActive(i <= health);
        }
    }
}
