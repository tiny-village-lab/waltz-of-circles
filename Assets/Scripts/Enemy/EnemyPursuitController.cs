using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPursuitController : MonoBehaviour
{

    private float speed = 3;

    // The configured speed with random variations
    private float variableSpeed;

    private float speedVariationFrequency = 0.1f;

    private float speedVariationAmplitude = 3.4f;

    private Rigidbody2D rb;

    private Transform player;

    public EnemyPursuitSpriteController sprite;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        speed = Random.Range(4.0f, 6.4f);
        speedVariationFrequency = Random.Range(0.03f, 1.4f);
        speedVariationAmplitude = Random.Range(1.2f, 6.0f);

        variableSpeed = speed;
        StartCoroutine(SpeedVariation());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = player.position - transform.position;

        transform.up = target;

        if (rb != null) {
            rb.AddForce(transform.up * variableSpeed);
        }
    }

    IEnumerator SpeedVariation()
    {
        bool isGoingFaster = true;

        while(true) {

            if (Mathf.Abs(speed - variableSpeed) >= speedVariationAmplitude) {
                isGoingFaster = !isGoingFaster;
            }

            if (isGoingFaster) {
                variableSpeed += speedVariationFrequency;
            } else {
                variableSpeed -= speedVariationFrequency;
            }

            yield return null;
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            sprite.Explode();

            LevelPursuitManager.instance.OneEnemyDestroyed();
            StopCoroutine(SpeedVariation());
        }
    }

    void Destroy()
    {
    }
}
