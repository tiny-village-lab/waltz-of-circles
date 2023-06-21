using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager instance;

    private Level level;
    private Levels levelsConfiguration;

    public TextAsset levelsSetupFile;

    private float worldWidth;
    private float worldHeight;

    private float positionOffsetForEnemiesAtSpawn = 2;

    public PlayerHealth playerHealth;

    public GameObject prefabEnemyA;
    public GameObject prefabEnemyB;
    public GameObject prefabEnemyC;

    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }

        instance = this;
    }

    void Start()
    {
        levelsConfiguration = JsonUtility.FromJson<Levels>(levelsSetupFile.text);
        level = levelsConfiguration.levels[0];    
        AudioManager.instance.SetProgression(level.number);
        
        AudioManager.instance.Bar += SpawnEnemiesOnBar;
        AudioManager.instance.Beat += SpawnEnemiesOnBeat;

        worldHeight = Camera.main.orthographicSize * 2;
        worldWidth = Camera.main.aspect * worldHeight;
    }

    void Update()
    {
        AudioManager.instance.SetIntensity(Intensity());
        AudioManager.instance.SetDanger(Danger());
    }

    private void SpawnEnemiesOnBar()
    {
        if (
            level.numberOfEnemiesA > 0 
            && level.numberOfEnemiesASpawned < level.numberOfEnemiesA
            && level.EnemiesTotalAlive() < level.enemyMaxInstances
        ) {
            level.numberOfEnemiesASpawned++;
            SpawnEnemy(prefabEnemyA);
        }

        if (
            level.numberOfEnemiesC > 0 
            && level.numberOfEnemiesCSpawned < level.numberOfEnemiesC
            && level.EnemiesTotalAlive() < level.enemyMaxInstances
        ) {
            level.numberOfEnemiesCSpawned++;
            SpawnEnemy(prefabEnemyC);
        }
    }

    private void SpawnEnemiesOnBeat()
    {
        for (int i=0; i<2; i++) {
            if (
                level.numberOfEnemiesB > 0 
                && level.numberOfEnemiesBSpawned < level.numberOfEnemiesB
                && level.EnemiesTotalAlive() < level.enemyMaxInstances
            ) {
                level.numberOfEnemiesBSpawned++;
                SpawnEnemy(prefabEnemyB);
            }
        }
    }

    private void SpawnEnemy(GameObject prefab)
    {
        float leftOrRight = level.EnemiesTotalSpawned() % 2 == 0 ? 1 : -1;
        float topOrBottom = Random.value < 0.5 ? 1 : -1;

        Vector3 target = new Vector3(
            Random.Range(-worldWidth, worldWidth) * leftOrRight,
            (Camera.main.orthographicSize + Random.Range(3, positionOffsetForEnemiesAtSpawn)) * topOrBottom, 
            0
        );

        level.activeEnemies.Add(
            Object.Instantiate(prefab, target, transform.rotation)
        );
    }

    private void LevelUp()
    {
        if (GameManager.instance.GameIsOver()) {
            return;
        }

        foreach (GameObject enemy in level.activeEnemies) {
            Destroy(enemy);
        }
        level.activeEnemies = new List<GameObject>();

        level = levelsConfiguration.PickNextLevel();
        AudioManager.instance.SetProgression(level.number % 6);
        AudioManager.instance.PlayFxLevelUp();
        
        GameManager.instance.EmitOnBreak();
        GameManager.instance.EmitLevelUp(level.number);
    }

    public Level GetCurrentLevel()
    {
        return level;
    }

    public void OneEnemyDestroyed()
    {
        GameManager.instance.OneEnemyDestroyed();
        level.enemyCountDestroyed++;

        if (level.enemyCountDestroyed >= level.EnemiesTotalExpected()) {
            LevelUp();
        }
    }

    private float GetClosestDistanceFromEnemy()
    {
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in level.activeEnemies) {

            if (enemy != null) {
                Vector3 directionToTarget = enemy.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if(dSqrToTarget < closestDistanceSqr) {
                    closestDistanceSqr = dSqrToTarget;
                }
            }
            
        }

        return closestDistanceSqr;
    }
    
    private int Intensity()
    {
        float shortestDistanceClamped = Mathf.Clamp(GetClosestDistanceFromEnemy(), 0, 1);

        float progression = (level.EnemiesTotalSpawned() - level.EnemiesTotalAlive()) / (float) level.EnemiesTotalExpected() * 100;

        int intensity = Mathf.RoundToInt(
            ((progression * 70) + (Danger() * 30)) / 100.0f
        );

        return intensity;
    }

    private int Danger()
    {
        return Mathf.RoundToInt(
            (5 - playerHealth.GetHealth()) / 5.0f * 100
        );
    }
}
