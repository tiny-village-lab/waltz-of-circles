using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    private float timeScale = 1;

    private bool stopTime = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnBreak += StopTime;
        GameManager.instance.OnUnbreak += StartTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (stopTime) {
            timeScale = 0;
        }
        if (!stopTime) {
            timeScale = 1;
        }

        Mathf.Clamp(timeScale, 0, 1);

        Time.timeScale = timeScale;
    }

    private void StopTime()
    {
        stopTime = true;
    }

    private void StartTime()
    {
        stopTime = false;
    }
}
