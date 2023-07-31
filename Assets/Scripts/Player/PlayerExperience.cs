using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExperience: MonoBehaviour
{

    private int points;

    void Start()
    {
        SetExperience(12);
    }

    public void SetExperience(int experiencePoints)
    {
        points = experiencePoints;
    }

    public int GetExperience()
    {
        return points;
    }

    public void AddExperience(int experiencePoints)
    {
        points += experiencePoints;
    }
}
