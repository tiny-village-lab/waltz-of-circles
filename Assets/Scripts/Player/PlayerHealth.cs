using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Handle the health of the player

This class carries the player health information.
*/

public class PlayerHealth : MonoBehaviour
{
    public HealthBar healthBar;

    public NextHeartCountdown nextHeartCountdown;

    // current health points
    private int health;

    // Max health points that the player can have
    private const int maxHealth = 4;

    // The number of Bars that will be counted before we add one 
    // point of health to the player
    private int nextBarsToWinAHealthPoint = 0;

    // Max number of Bars to wait
    private const int countDownDuration = 10;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        AudioManager.instance.Bar += ControleHeartCountdown;
        GameManager.instance.OnRestart += Init;
    }

    void Init()
    {
        health = maxHealth;
        healthBar.UpdateHealthBar(health);
        nextBarsToWinAHealthPoint = 0;
    }


    /*
     * If you arrived at the end of the countdown, you earn + 1 health
     * If are not arrived at the end of the countdown, but the pursuit mode is on, you win
     * the health point
     */
    private void ControleHeartCountdown()
    {
        if (GameManager.instance.features.healthRegenerate == false) {
            return;
        }

        if (health == maxHealth) {
            return;
        }

        if (GameManager.instance.IsOnPursuitMode() && nextBarsToWinAHealthPoint > 0) {
            nextBarsToWinAHealthPoint = 0;
            PlusOneHealth();
            healthBar.UpdateHealthBar(health);
            return;
        }

        if (nextBarsToWinAHealthPoint > 0) {
            nextBarsToWinAHealthPoint--;

            if (nextBarsToWinAHealthPoint == 0) {
                PlusOneHealth();
                healthBar.UpdateHealthBar(health);

                if (health != maxHealth) {
                    nextBarsToWinAHealthPoint = countDownDuration;
                }
            }
        }

        nextHeartCountdown.UpdateCountdownText(nextBarsToWinAHealthPoint);
    }

    public void MinusOneHealth()
    {
        health--;
        healthBar.UpdateHealthBar(health);

        if (health < 0) {
            GameManager.instance.GameOver();
        }

        if (GameManager.instance.IsOnPursuitMode()) {
            return;
        }

        nextBarsToWinAHealthPoint = countDownDuration;
    }

    private void PlusOneHealth()
    {
        if (health == maxHealth) {
            return;
        }

        health++;
    }

    public int GetHealth()
    {
        return health;
    }
}
