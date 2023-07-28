using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    public static WaveManager instance;

    private Wave wave;
    private Waves wavesConfiguration;

    public TextAsset wavesSetupFile;

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
        wavesConfiguration = JsonUtility.FromJson<Waves>(wavesSetupFile.text);
        wave = wavesConfiguration.waves[0];    
        AudioManager.instance.SetProgression(wave.number);
        
        AudioManager.instance.Bar += SpawnEnemiesOnBar;
        AudioManager.instance.Beat += SpawnEnemiesOnBeat;

        worldHeight = Camera.main.orthographicSize * 2;
        worldWidth = Camera.main.aspect * worldHeight;

        GameManager.instance.ZoomIn();
    }

    void Update()
    {
        AudioManager.instance.SetIntensity(Intensity());
        AudioManager.instance.SetDanger(Danger());

        // If there is only one enemy that remains, we destroy it ourselves
        if (wave.EnemiesTotalSpawned() == wave.EnemiesTotalExpected() && wave.EnemiesTotalAlive() == 1) {
            DestroyAllEnemies();    
        }
    }

    private void SpawnEnemiesOnBar()
    {
        if (
            wave.numberOfEnemiesA > 0 
            && wave.numberOfEnemiesASpawned < wave.numberOfEnemiesA
            && wave.EnemiesTotalAlive() < wave.enemyMaxInstances
        ) {
            wave.numberOfEnemiesASpawned++;
            SpawnEnemy(prefabEnemyA);
        }

        if (
            wave.numberOfEnemiesC > 0 
            && wave.numberOfEnemiesCSpawned < wave.numberOfEnemiesC
            && wave.EnemiesTotalAlive() < wave.enemyMaxInstances
        ) {
            wave.numberOfEnemiesCSpawned++;
            SpawnEnemy(prefabEnemyC);
        }
    }

    private void SpawnEnemiesOnBeat()
    {
        for (int i=0; i<2; i++) {
            if (
                wave.numberOfEnemiesB > 0 
                && wave.numberOfEnemiesBSpawned < wave.numberOfEnemiesB
                && wave.EnemiesTotalAlive() < wave.enemyMaxInstances
            ) {
                wave.numberOfEnemiesBSpawned++;
                SpawnEnemy(prefabEnemyB);
            }
        }
    }

    private void SpawnEnemy(GameObject prefab)
    {
        float leftOrRight = wave.EnemiesTotalSpawned() % 2 == 0 ? 1 : -1;
        float topOrBottom = Random.value < 0.5 ? 1 : -1;

        Vector3 target = new Vector3(
            Random.Range(-worldWidth, worldWidth) * leftOrRight,
            (Camera.main.orthographicSize + Random.Range(3, positionOffsetForEnemiesAtSpawn)) * topOrBottom, 
            0
        );

        wave.activeEnemies.Add(
            Object.Instantiate(prefab, target, transform.rotation)
        );
    }

    private void WaveUp()
    {
        if (wave.number == 1) {
            GameSceneManager.instance.LoadNextScene();
            return;
        }

        if (GameManager.instance.GameIsOver()) {
            return;
        }

        DestroyAllEnemies();

        wave = wavesConfiguration.PickNextWave();
        AudioManager.instance.SetProgression(wave.number % 6);
        AudioManager.instance.PlayFxWaveUp();
        
        GameManager.instance.EmitOnBreak();
        GameManager.instance.EmitWaveUp(wave.number);
    }

    private void DestroyAllEnemies()
    {
        foreach (GameObject enemy in wave.activeEnemies) {
            Destroy(enemy);
        }

        wave.activeEnemies = new List<GameObject>();
    }

    public Wave GetCurrentWave()
    {
        return wave;
    }

    public void OneEnemyDestroyed()
    {
        GameManager.instance.OneEnemyDestroyed();
        wave.enemyCountDestroyed++;

        if (wave.enemyCountDestroyed >= wave.EnemiesTotalExpected()) {
            WaveUp();
        }
    }

    private float GetClosestDistanceFromEnemy()
    {
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in wave.activeEnemies) {

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

        float progression = (wave.EnemiesTotalSpawned() - wave.EnemiesTotalAlive()) / (float) wave.EnemiesTotalExpected() * 100;

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

    void OnDestroy()
    {
        AudioManager.instance.Bar -= SpawnEnemiesOnBar;
        AudioManager.instance.Beat -= SpawnEnemiesOnBeat;
    }
}
