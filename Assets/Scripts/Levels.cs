using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Levels
{
    public Level[] levels;

    private int levelIndex = 0;

    public Level PickNextLevel()
    {
        levelIndex++;
        return levels[levelIndex];
    }
}