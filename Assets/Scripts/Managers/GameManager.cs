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

    public event System.Action OnGhostModeOn;
    public event System.Action OnGhostModeOff;
    private bool isGhostModeOn = false;

    private int gameOn = 0;

    private int score = 0;

    private bool isZooming = false;

    private bool isOnPursuitMode = false;

    public event System.Action OnPlayerCollideAWall;

    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this);
            return;
        }

        OnBreak += OnBreakListen;
        OnUnbreak += OnUnbreakListen;

        instance = this;
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

    public void OneEnemyDestroyed()
    {
        score++;
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


        if ((gameIsOnBreak || isGhostModeOn) && gameOn > 0) {
            gameOn -= 1;
        }

        if (!gameIsOnBreak && !isGhostModeOn && gameOn < 20) {
            gameOn += 1;
        }

        if (!isZooming) {
            StopCoroutine(ZoomOutRoutine());
        }

        AudioManager.instance.SetGameOn(gameOn);
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

    public void GhostModeOn()
    {
        isGhostModeOn = true;
        OnGhostModeOn?.Invoke();
    }

    public void GhostModeOff()
    {
        isGhostModeOn = false;
        OnGhostModeOff?.Invoke();
    }

    public bool IsGhostModeOn()
    {
        return isGhostModeOn;
    }

    public void EmitOnBreak()
    {
        OnBreak?.Invoke();
    }

    public void EmitLevelUp(int levelNumber)
    {
        OnLevelUp?.Invoke(levelNumber);
    }

    public void SetPursuitModeOn()
    {
        isOnPursuitMode = true;
    }

    public bool IsOnPursuitMode()
    {
        return isOnPursuitMode;
    }

    public void ZoomOut()
    {
        isZooming = true;

        StartCoroutine(ZoomOutRoutine());
    }

    IEnumerator ZoomOutRoutine()
    {
        float zoom = Camera.main.orthographicSize;

        while (isZooming == true) {
            zoom += 0.02f;
            Camera.main.orthographicSize = zoom;

            if (Camera.main.orthographicSize >= 5) {
                isZooming = false;
            }

            yield return null;
        }
    }

    public void PlayerCollideAWall()
    {
        OnPlayerCollideAWall?.Invoke();
    }
}
