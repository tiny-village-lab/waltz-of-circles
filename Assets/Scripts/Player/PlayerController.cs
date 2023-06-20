using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private CircleCollider2D circleCollider;

    private bool isUntouchable = false;
    private float nextTimeIsTouchable = 0.0f;
    private float untouchableDuration = 2.0f;

    public AnimateSpriteInRythm animateSpriteInRythm;

    private bool isGhost = false;

    public PlayerHealth playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();

        GameManager.instance.OnGhostModeOn += StartGhost;
        GameManager.instance.OnGhostModeOff += StopGhost;
    }

    void Update()
    {
        if (GameManager.instance.GameIsOver()) {
            return;
        }
        
        if (isUntouchable && nextTimeIsTouchable > 0f) {
            nextTimeIsTouchable -= Time.deltaTime;
        }

        if (nextTimeIsTouchable <= 0) {
            isUntouchable = false;
            animateSpriteInRythm.IsTouchable();
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
