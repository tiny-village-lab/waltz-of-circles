using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPursuitManager : MonoBehaviour
{

    public static LevelPursuitManager instance;
    
    public GameObject prefabEnemy;

    private int numberOfEnemiesToSpawn = 12;
    private Level level;
    private Levels levelsConfiguration;


    private float worldWidth;
    private float worldHeight;

    private float positionOffsetForEnemiesAtSpawn = 2;

    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        worldHeight = Camera.main.orthographicSize * 2;
        worldWidth = Camera.main.aspect * worldHeight;

        level = new Level();
        level.numberOfEnemiesA = 12;
        level.enemyMaxInstances = 3;

        AudioManager.instance.Beat += SpawnEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemy()
    {
        if (level.EnemiesTotalSpawned() >= level.EnemiesTotalExpected()) {
            return;
        }

        if (level.EnemiesTotalAlive() >= level.enemyMaxInstances) {
            return;
        }

        level.numberOfEnemiesASpawned++;
        level.activeEnemies.Add(
            Object.Instantiate(prefabEnemy, SpawnPosition(), transform.rotation)
        );
    }

    Vector3 SpawnPosition()
    {
        return new Vector3(
            -worldWidth - positionOffsetForEnemiesAtSpawn,
            Random.Range(-worldHeight, worldHeight),
            0
        );
    }

    public void OneEnemyDestroyed()
    {
        GameManager.instance.OneEnemyDestroyed();
        level.enemyCountDestroyed++;
    }
}
