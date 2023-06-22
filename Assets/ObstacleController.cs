using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{

    private float speed = 20.0f;

    private Vector3 initialPosition;

    // When an obstacle spawns, it has to stay put so we can warn the player
    // that an obstacle is coming
    private bool move = false;

    // Number of beats to count before moving
    private int beatsToMove = 7;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        AudioManager.instance.Beat += CountBeats;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -28.0f) {
            AudioManager.instance.Beat -= CountBeats;
            Destroy(gameObject);
        }

        if (move) {
            transform.Translate(Vector3.left * speed * Time.deltaTime);        
        }
    }

    void CountBeats()
    {
        if (beatsToMove == 0) {
            move = true;
            return;
        }

        beatsToMove--;
    }
}
