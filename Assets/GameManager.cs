using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance { get ; private set; }

    private float worldWidth;
    private float worldHeight;

    private float positionOffsetForEnemiesAtSpawn = 2;

    public GameObject prefabEnemyA;
    private int enemyCountInstances = 0;

    public TextAsset levelsSetupFile;
    

    [System.Serializable]
    class Level
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

    private Level level;
    private Levels levelsConfiguration;

    void Awake()
    {
        levelsConfiguration = JsonUtility.FromJson<Levels>(levelsSetupFile.text);
        level = levelsConfiguration.levels[0];    
        AudioManager.instance.SetLevel(level.number);
        AudioManager.instance.Bar += AddOneToBarsCount;
        AudioManager.instance.Bar += SpawnEnemyA;

        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }
        instance = this;

    }

    private void LevelUp()
    {
        level = levelsConfiguration.PickNextLevel();
        level.barsCountInCurrentLevel = 0;
        AudioManager.instance.SetLevel(level.number);
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

            Object.Instantiate(prefabEnemyA, target, transform.rotation);
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
}
