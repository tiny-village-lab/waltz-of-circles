using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemySeekForPlayer : MonoBehaviour
{

    public float pushForce = 200.0f;

    private Rigidbody2D rb;

    // Target
    private Transform player;

    public enum MoveEveryOptions {Bar, Beat};
    public MoveEveryOptions moveEvery;

    void Awake()
    {
        if (AudioManager.instance == null) {
            return;
        }

        if (moveEvery == MoveEveryOptions.Bar) {
            AudioManager.instance.Bar += MakeAStep;
        }

        if (moveEvery == MoveEveryOptions.Beat) {
            AudioManager.instance.Beat += MakeAStep;
        }
    }

    void Destroy()
    {
        Unsubscribe();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = (GameObject.Find("Player")).transform;
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
        if (GameManager.instance.GameIsOnBreak()) {
            return;
        }

        if (rb != null) {
            rb.AddForce(transform.up * pushForce);
        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("EnemyA")) {
            GameManager.instance.OneEnemyDestroyed(gameObject.tag);
            Unsubscribe();
            Destroy(this.gameObject);
        }
    }

    void Unsubscribe()
    {
        if (AudioManager.instance != null) {
            AudioManager.instance.Bar -= MakeAStep;
            AudioManager.instance.Beat -= MakeAStep;
        }
    }
}