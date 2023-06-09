using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private float playerSpeed = 2.0f;
    private PlayerInput playerInput;
    private Rigidbody2D rb;

    private Vector2 movement;

    private float reboundForce = 3.0f;

    public HealthBar healthBar;

    private int health = 4;

    private bool isUntouchable = false;
    private float nextTimeIsTouchable = 0.0f;
    private float untouchableDuration = 2.0f;

    private int nextBarsToWinAHeart = 0;
    private int durationInBarsToWaitToWinAHeart = 6;

    public AnimateSpriteInRythm animateSpriteInRythm;

    public NextHeartCountdown nextHeartCountdown;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        healthBar.SetHealth(health);

        AudioManager.instance.Bar += ControleHeartCountdown;
    }

    void Update()
    {
        movement = playerInput.actions["Move"].ReadValue<Vector2>();
        PreventPlayerToGoOffScreen();

        if (isUntouchable && nextTimeIsTouchable > 0f) {
            nextTimeIsTouchable -= Time.deltaTime;
        }

        if (nextTimeIsTouchable <= 0) {
            isUntouchable = false;
            animateSpriteInRythm.IsTouchable();
        }

        if (health < 4 && nextBarsToWinAHeart == 0) {
            nextBarsToWinAHeart = durationInBarsToWaitToWinAHeart;
        }
    }

    void PreventPlayerToGoOffScreen()
    {
        // Convert the player world position to player screen position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPosition.x < 0) {
            movement = new Vector2(
                -movement.x + reboundForce,
                movement.y
            );
        }

        if (screenPosition.x > Camera.main.pixelWidth) {
            movement = new Vector2(
                -movement.x - reboundForce,
                movement.y
            );
        }

        if (screenPosition.y < 0) {
            movement = new Vector2(
                movement.x,
                -movement.y + reboundForce
            );
        }

        if (screenPosition.y > Camera.main.pixelHeight) {
            movement = new Vector2(
                movement.x,
                movement.y - reboundForce
            );
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(movement * playerSpeed, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (isUntouchable) {
            return;
        }

        AudioManager.instance.PlayFxPlayerHit(transform.position);

        if (other.gameObject.CompareTag("EnemyA") == false) {
            return;
        }

        if (health == 0) {
            GameManager.instance.GameOver();
            return;
        }

        health--;
        healthBar.SetHealth(health);
        
        isUntouchable = true;
        nextTimeIsTouchable = untouchableDuration;
        animateSpriteInRythm.IsUntouchable();

        nextBarsToWinAHeart = durationInBarsToWaitToWinAHeart;
    }

    private void ControleHeartCountdown()
    {
        if (nextBarsToWinAHeart > 0) {
            nextBarsToWinAHeart--;

            if (nextBarsToWinAHeart == 0) {
                health++;
                healthBar.SetHealth(health);
            }
        }

        nextHeartCountdown.UpdateCountdownText(nextBarsToWinAHeart);
    }
}
