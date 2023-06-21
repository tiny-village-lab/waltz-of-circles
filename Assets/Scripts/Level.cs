using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public List<GameObject> activeEnemies = new List<GameObject>();

    public Level()
    {
        
    }


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