using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get ; private set; }

    public event System.Action<int> OnLevelUp;

    public event System.Action OnBreak;

    public event System.Action OnUnbreak;

    private bool gameIsOnBreak;

    private float breakDuration = 3.0f;
    private float nextTimeToUnbreak = 0.0f;

    private int gameOn = 0;

    private float worldWidth;
    private float worldHeight;

    private float positionOffsetForEnemiesAtSpawn = 2;

    public GameObject prefabEnemyA;

    private List<GameObject> activeEnemies;

    private int enemyCountInstances = 0;

    public TextAsset levelsSetupFile;

    [System.Serializable]
    public class Level
    {
        public int number = 0;

        public int barsCountInCurrentLevel = 0 ;

        // Number of enemies destroyed
        public int enemyCountDestroyed = 0;

        public int enemyCountDestroyedThreshold;
        public int enemyMaxInstances;
    }

    [System.Serializable]
    class Levels
    {
        public Level[] levels;

        private int levelIndex = 0;

        public Level PickNextLevel()
        {
            levelIndex++;
            return levels[levelIndex];
        }
    }

    public Level level;
    private Levels levelsConfiguration;

    void Awake()
    {
        levelsConfiguration = JsonUtility.FromJson<Levels>(levelsSetupFile.text);
        level = levelsConfiguration.levels[0];    
        AudioManager.instance.SetProgression(level.number);
        AudioManager.instance.Bar += AddOneToBarsCount;
        AudioManager.instance.Bar += SpawnEnemyA;

        activeEnemies = new List<GameObject>();

        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }

        OnBreak += OnBreakListen;
        OnUnbreak += OnUnbreakListen;

        instance = this;
    }

    private void LevelUp()
    {
        level = levelsConfiguration.PickNextLevel();
        level.barsCountInCurrentLevel = 0;
        activeEnemies = new List<GameObject>();
        AudioManager.instance.SetProgression(level.number);
        
        OnBreak?.Invoke();

        OnLevelUp?.Invoke(level.number);
    }

    private void AddOneToBarsCount()
    {
        level.barsCountInCurrentLevel++;
    }

    private void SpawnEnemyA()
    {
        if (level.enemyMaxInstances == enemyCountInstances) {
            return;
        }

        if (level.barsCountInCurrentLevel % 2 == 0) {

            float leftOrRight = Random.value < 0.5 ? 1 : -1;
            float topOrBottom = Random.value < 0.5 ? 1 : -1;

            Vector3 target = new Vector3(
                Random.Range(-worldWidth, worldWidth) * leftOrRight,
                (Camera.main.orthographicSize + Random.Range(3, positionOffsetForEnemiesAtSpawn)) * topOrBottom, 
                0
            );

            activeEnemies.Add(
                Object.Instantiate(prefabEnemyA, target, transform.rotation)
            );

            enemyCountInstances++;
        }
    }

    public void OneEnemyDestroyed(string tag)
    {
        enemyCountInstances--;
        level.enemyCountDestroyed++;

        if (level.enemyCountDestroyed >= level.enemyCountDestroyedThreshold) {
            LevelUp();
        }
    }


    void Start()
    {
        worldHeight = Camera.main.orthographicSize * 2;
        worldWidth = Camera.main.aspect * worldHeight;
    }

    void Update()
    {
        if (nextTimeToUnbreak >= 0) {
            nextTimeToUnbreak -= Time.unscaledDeltaTime;
        }

        if (nextTimeToUnbreak < 0) {
            OnUnbreak?.Invoke();
        }

        AudioManager.instance.SetIntensity(
            Mathf.RoundToInt(
                //enemyCountInstances / 6 * 100
                Intensity()
            )
        );

        if (gameIsOnBreak && gameOn > 0) {
            gameOn -= 1;
        }

        if (!gameIsOnBreak && gameOn < 20) {
            gameOn += 1;
        }

        AudioManager.instance.SetGameOn(gameOn);
    }

    private float GetClosestDistanceFromEnemy()
    {
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in activeEnemies) {

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

        int intensity = Mathf.RoundToInt(
            ((enemyCountInstances + level.enemyCountDestroyed) / level.enemyCountDestroyedThreshold * 100) * shortestDistanceClamped
        );

        return intensity;
    }

    private void OnBreakListen()
    {
        gameIsOnBreak = true;

        nextTimeToUnbreak = breakDuration;
    }

    private void OnUnbreakListen()
    {
        gameIsOnBreak = false;
    }

    public bool GameIsOnBreak()
    {
        return gameIsOnBreak;
    }
}
