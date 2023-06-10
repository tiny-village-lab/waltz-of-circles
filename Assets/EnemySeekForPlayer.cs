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

    public AnimateSpriteInRythm animateSpriteInRythm;

    public bool leaveBodyOnDeath;
    private bool isDead = false;

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
        if (isDead) {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation
                | RigidbodyConstraints2D.FreezePositionY
                | RigidbodyConstraints2D.FreezePositionX;
            return;
        }

        Vector3 target = player.position - transform.position;

        // Look at the player
        transform.up = target;
    }

    void FixedUpdate()
    {
    }

    private void MakeAStep()
    {
        if (GameManager.instance.GameIsOnBreak() || GameManager.instance.GameIsOver()) {
            return;
        }

        if (rb != null) {
            rb.AddForce(transform.up * pushForce);
        }

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("EnemyA") == false) {
            return;
        }

        GameManager.instance.OneEnemyDestroyed(gameObject.tag);
        Unsubscribe();

        if (leaveBodyOnDeath) {
            isDead = true;
            animateSpriteInRythm.SetIsDead();
            return;
        }

        gameObject.SetActive(false);
    }

    void Unsubscribe()
    {
        if (AudioManager.instance != null) {
            AudioManager.instance.Bar -= MakeAStep;
            AudioManager.instance.Beat -= MakeAStep;
        }
    }

    public bool IsDead()
    {
        return isDead;
    }
}
