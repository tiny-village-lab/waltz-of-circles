using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get ; private set; }

    public event System.Action<int> OnLevelUp;
    public event System.Action OnGameOver;

    private bool gameOver = false;

    public event System.Action OnBreak;

    public event System.Action OnUnbreak;

    private bool gameIsOnBreak;

    private float breakDuration = 3.0f;
    private float nextTimeToUnbreak = 0.0f;

    public event System.Action OnTeleportModeOn;
    public event System.Action OnTeleportModeOff;
    private bool isTeleportModeOn = false;
    private int countTeleportModeOn = 0;

    private int gameOn = 0;

    private float worldWidth;
    private float worldHeight;

    private float positionOffsetForEnemiesAtSpawn = 2;

    public GameObject prefabEnemyA;
    public GameObject prefabEnemyB;
    public GameObject prefabEnemyC;

    private List<GameObject> activeEnemies;

    public TextAsset levelsSetupFile;

    public HealthBar healthBar;

    private int score = 0;

    [System.Serializable]
    public class Level
    {
        public int number = 0;

        // Number of enemies destroyed
        public int enemyCountDestroyed = 0;

        public int enemyMaxInstances;


        public int numberOfEnemiesA;
        
        public int numberOfEnemiesB;

        public int numberOfEnemiesC;

        public int numberOfEnemiesASpawned = 0;
        public int numberOfEnemiesBSpawned = 0;
        public int numberOfEnemiesCSpawned = 0;

        public int EnemiesTotalExpected()
        {
            return numberOfEnemiesA
                + numberOfEnemiesB
                + numberOfEnemiesC;
        }

        public int EnemiesTotalSpawned()
        {
            return numberOfEnemiesASpawned
                + numberOfEnemiesBSpawned
                + numberOfEnemiesCSpawned;
        }
        public int EnemiesTotalAlive()
        {
            return EnemiesTotalSpawned() - enemyCountDestroyed;
        }
    }

    [System.Serializable]
    class Levels
    {
        public Level[] levels;

        private int levelIndex = 0;

        public Level PickNextLevel()
        {
            levelIndex++;
            if (levelIndex == 7) {
                levelIndex = 1;
            }
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
        AudioManager.instance.Bar += SpawnEnemiesOnBar;
        AudioManager.instance.Beat += SpawnEnemiesOnBeat;
        AudioManager.instance.Beat += ControleTeleportModeOn;

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
        if (gameOver) {
            return;
        }

        foreach (GameObject enemy in activeEnemies) {
            Destroy(enemy);
        }
        activeEnemies = new List<GameObject>();

        level = levelsConfiguration.PickNextLevel();
        AudioManager.instance.SetProgression(level.number % 6);
        AudioManager.instance.PlayFxLevelUp();
        
        OnBreak?.Invoke();

        OnLevelUp?.Invoke(level.number);
    }

    public void GameOver()
    {
        gameOver = true;
        AudioManager.instance.SetProgression(9);
        AudioManager.instance.SetIntensity(0);
        OnGameOver?.Invoke();
    }

    public bool GameIsOver()
    {
        return gameOver;
    }

    private void SpawnEnemiesOnBar()
    {
        for (int i=0; i<2; i++) {
            if (
                level.numberOfEnemiesA > 0 
                && level.numberOfEnemiesASpawned < level.numberOfEnemiesA
                && level.EnemiesTotalAlive() < level.enemyMaxInstances
            ) {
                SpawnEnemy(prefabEnemyA);
                level.numberOfEnemiesASpawned++;
            }

            if (
                level.numberOfEnemiesC > 0 
                && level.numberOfEnemiesCSpawned < level.numberOfEnemiesC
                && level.EnemiesTotalAlive() < level.enemyMaxInstances
            ) {
                SpawnEnemy(prefabEnemyC);
                level.numberOfEnemiesCSpawned++;
            }
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
                SpawnEnemy(prefabEnemyB);
                level.numberOfEnemiesBSpawned++;
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

        activeEnemies.Add(
            Object.Instantiate(prefab, target, transform.rotation)
        );
    }

    public void OneEnemyDestroyed(string tag)
    {
        level.enemyCountDestroyed++;
        score++;

        if (level.enemyCountDestroyed >= level.EnemiesTotalExpected()) {
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
        if (gameOver) {
            return;
        }

        if (nextTimeToUnbreak >= 0) {
            nextTimeToUnbreak -= Time.unscaledDeltaTime;
        }

        if (nextTimeToUnbreak < 0) {
            OnUnbreak?.Invoke();
        }

        AudioManager.instance.SetIntensity(Intensity());

        AudioManager.instance.SetDanger(Danger());

        if ((gameIsOnBreak || isTeleportModeOn) && gameOn > 0) {
            gameOn -= 1;
        }

        if (!gameIsOnBreak && !isTeleportModeOn && gameOn < 20) {
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

        float progression = (level.EnemiesTotalSpawned() - level.EnemiesTotalAlive()) / (float) level.EnemiesTotalExpected() * 100;

        int intensity = Mathf.RoundToInt(
            ((progression * 70) + (Danger() * 30)) / 100.0f
        );

        return intensity;
    }

    private int Danger()
    {
        return Mathf.RoundToInt(
            (5 - healthBar.GetHealth()) / 5.0f * 100
        );
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

    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public int GetScore()
    {
        return score;
    }

    public void TeleportModeOn()
    {
        isTeleportModeOn = true;
        OnTeleportModeOn?.Invoke();
    }

    public void TeleportModeOff()
    {
        countTeleportModeOn = 0;
        isTeleportModeOn = false;
        OnTeleportModeOff?.Invoke();
    }

    private void ControleTeleportModeOn()
    {
        if (!isTeleportModeOn) {
            return;
        }

        countTeleportModeOn++;

        if (countTeleportModeOn == 4) {
            TeleportModeOff();
        }
    }

    public bool IsTeleportModeOn()
    {
        return isTeleportModeOn;
    }
}
