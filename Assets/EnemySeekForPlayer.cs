using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySeekForPlayer : MonoBehaviour
{

    public float pushForce = 200.0f;

    private Rigidbody2D rb;

    // Target
    public Transform player;

    void Awake()
    {
        if (AudioManager.instance != null) {
            AudioManager.instance.Bar += MakeAStep;
        }
    }

    void Destroy()
    {
        if (AudioManager.instance != null) {
            AudioManager.instance.Bar -= MakeAStep;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = player.position - transform.position;

        // Look at the player
        transform.up = target;
    }

    void FixedUpdate()
    {
    }

    private void MakeAStep()
    {
        rb.AddForce(transform.up * pushForce);
    }
}
