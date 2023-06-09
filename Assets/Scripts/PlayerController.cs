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

    private int health = 5;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
        healthBar.SetHealth(health);
    }

    void Update()
    {
        movement = playerInput.actions["Move"].ReadValue<Vector2>();
        PreventPlayerToGoOffScreen();
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
        if (other.gameObject.CompareTag("EnemyA")) {
            health--;
            healthBar.SetHealth(health);
        }
    }
}
