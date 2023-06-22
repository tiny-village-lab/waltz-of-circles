using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{

    public float playerSpeed;
    
    private PlayerInput playerInput;

    private Rigidbody2D rb;

    private Vector2 movement;

    private float reboundForce = 3.0f;


    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.GameIsOver()) {
            return;
        }
        
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
}
