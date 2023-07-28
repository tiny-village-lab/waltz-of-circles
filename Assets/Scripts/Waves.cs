using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waves
{
    public Wave[] waves;

    private int waveIndex = 0;

    public Wave PickNextWave()
    {
        waveIndex++;
        return waves[waveIndex];
    }
}