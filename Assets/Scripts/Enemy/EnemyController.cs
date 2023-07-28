using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
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

    private bool isOffScreen = true;

    private Vector3 lastRecordedPlayerPosition;

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
        lastRecordedPlayerPosition = player.position;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIsOffScreen();
        
        if (isDead) {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation
                | RigidbodyConstraints2D.FreezePositionY
                | RigidbodyConstraints2D.FreezePositionX;
            return;
        }

        if (GameManager.instance.IsGhostModeOn() == false) {
            lastRecordedPlayerPosition = player.position;
        }
        
        Vector3 target = lastRecordedPlayerPosition - transform.position;

        // Look at the player
        transform.up = target;
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
        if (isOffScreen) {
            return;
        }

        if (other.gameObject.CompareTag("EnemyA") == false) {
            return;
        }

        AudioManager.instance.PlayFxEnemyHit();

        WaveManager.instance.OneEnemyDestroyed();
        Unsubscribe();

        isDead = true;

        if (leaveBodyOnDeath) {
            animateSpriteInRythm.SetIsDead();
            return;
        }

        animateSpriteInRythm.SetExplode();
    }

    void CheckIsOffScreen()
    {
        if (isOffScreen == false) {
            return;
        }

        // Convert the player world position to player screen position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        if (
            screenPosition.x > 0
            && screenPosition.x < Camera.main.pixelWidth
            && screenPosition.y > 0
            && screenPosition.y < Camera.main.pixelHeight
        ) {
            isOffScreen = false;
        }
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
