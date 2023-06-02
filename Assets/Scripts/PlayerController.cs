using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private Vector3 playerVelocity;
    private float playerSpeed = 50.0f;
    private PlayerInput playerInput;
    private Rigidbody2D rb;

    private Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movement = playerInput.actions["Move"].ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        rb.AddForce(movement * playerSpeed);
    }
}
