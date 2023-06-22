using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPursuitManager : MonoBehaviour
{

    public static LevelPursuitManager instance;
    
    public GameObject prefabEnemy;

    public GameObject prefabObstacle;

    private int spawnObstacleEvery = 16;
    private int spawnObstacleIn;

    private Level level;
    private Levels levelsConfiguration;


    private float worldWidth;
    private float worldHeight;

    private float positionOffsetForEnemiesAtSpawn = 0.5f;

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
        spawnObstacleIn = spawnObstacleEvery;

        worldHeight = Camera.main.orthographicSize * 2;
        worldWidth = Camera.main.aspect * worldHeight;

        level = new Level();
        level.numberOfEnemiesA = 12;
        level.enemyMaxInstances = 3;

        AudioManager.instance.Beat += SpawnEnemy;
        AudioManager.instance.Beat += SpawnObstacle;
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
            Object.Instantiate(prefabEnemy, EnemySpawnPosition(), transform.rotation)
        );
    }

    Vector3 EnemySpawnPosition()
    {
        return new Vector3(
            -worldWidth - positionOffsetForEnemiesAtSpawn,
            Random.Range(-worldHeight, worldHeight),
            0
        );
    }

    Vector3 ObstacleSpawnPosition()
    {
        return new Vector3(
            worldWidth / 2 + 0.5f,
            Random.Range(-2.0f, 2.0f),
            0
        );
    }

    public void OneEnemyDestroyed()
    {
        GameManager.instance.OneEnemyDestroyed();
        level.enemyCountDestroyed++;
    }

    void SpawnObstacle()
    {
        if (spawnObstacleIn != 0) {
            spawnObstacleIn--;
            return;
        }

        Object.Instantiate(prefabObstacle, ObstacleSpawnPosition(), transform.rotation);
        spawnObstacleIn = spawnObstacleEvery;
    }
}
