using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private float playerSpeed = 2.0f;
    private PlayerInput playerInput;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    private Vector2 movement;

    private float reboundForce = 3.0f;

    private bool isUntouchable = false;
    private float nextTimeIsTouchable = 0.0f;
    private float untouchableDuration = 2.0f;

    public AnimateSpriteInRythm animateSpriteInRythm;

    private bool isGhost = false;

    public PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();

        GameManager.instance.OnGhostModeOn += StartGhost;
        GameManager.instance.OnGhostModeOff += StopGhost;
    }

    void Update()
    {
        if (GameManager.instance.GameIsOver()) {
            return;
        }
        
        movement = playerInput.actions["Move"].ReadValue<Vector2>();
        PreventPlayerToGoOffScreen();

        if (isUntouchable && nextTimeIsTouchable > 0f) {
            nextTimeIsTouchable -= Time.deltaTime;
        }

        if (nextTimeIsTouchable <= 0) {
            isUntouchable = false;
            animateSpriteInRythm.IsTouchable();
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

    void StartGhost()
    {
        isGhost = true;
        circleCollider.enabled = false;
        animateSpriteInRythm.SetIsGhost();
    }

    void StopGhost()
    {
        isGhost = false;
        circleCollider.enabled = true;
        animateSpriteInRythm.SetIsNotGhost();
    }

    void FixedUpdate()
    {
        rb.AddForce(movement * playerSpeed, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (isUntouchable || isGhost) {
            return;
        }

        if (other.gameObject.CompareTag("EnemyA") == false) {
            return;
        }

        if (other.gameObject.GetComponent<EnemyController>().IsDead()) {
            return;
        }

        AudioManager.instance.PlayFxPlayerHit(transform.position);

        playerHealth.MinusOneHealth();
        
        isUntouchable = true;
        nextTimeIsTouchable = untouchableDuration;
        animateSpriteInRythm.IsUntouchable();
    }
}
