using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePursuitManager : MonoBehaviour
{

    public static WavePursuitManager instance;
    
    public GameObject prefabEnemy;

    public GameObject prefabObstacle;

    public ObstacleWarningController obstacleWarningController;

    private int spawnObstacleEvery = 16;
    private int spawnObstacleIn;

    private Wave wave;
    private Waves wavesConfiguration;


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

        wave = new Wave();
        wave.numberOfEnemiesA = 12;
        wave.enemyMaxInstances = 3;

        AudioManager.instance.Beat += SpawnEnemy;
        AudioManager.instance.Beat += SpawnObstacle;

        GameManager.instance.ZoomOut();
    }

    void SpawnEnemy()
    {
        if (wave.EnemiesTotalSpawned() >= wave.EnemiesTotalExpected()) {
            return;
        }

        if (wave.EnemiesTotalAlive() >= wave.enemyMaxInstances) {
            return;
        }

        wave.numberOfEnemiesASpawned++;
        wave.activeEnemies.Add(
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
            worldWidth / 2 + 5.4f,
            Random.Range(-2.0f, 2.0f),
            0
        );
    }

    public void OneEnemyDestroyed()
    {
        GameManager.instance.OneEnemyDestroyed();
        wave.enemyCountDestroyed++;
    }

    void SpawnObstacle()
    {
        if (spawnObstacleIn != 0) {
            spawnObstacleIn--;
            return;
        }

        Vector3 obstaclePosition = ObstacleSpawnPosition();

        obstacleWarningController.SetYPosition(Camera.main.WorldToScreenPoint(obstaclePosition).y); 
        obstacleWarningController.Show();

        Object.Instantiate(prefabObstacle, obstaclePosition, transform.rotation);
        spawnObstacleIn = spawnObstacleEvery;
    }

    void OnDestroy()
    {
        AudioManager.instance.Beat -= SpawnEnemy;
        AudioManager.instance.Beat -= SpawnObstacle;
    }
}