using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{

    public static GameSceneManager instance;

    private int sceneIndex = 0;

    AsyncOperation loadingOperation;

    void Awake()
    {
        if (!instance) {
            instance = this;
        }
    }

    void Start()
    {
        GameManager.instance.OnRestart += LoadFirstScene;
    }

    public void LoadNextScene()
    {
        sceneIndex++;
        loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
    }

    public void LoadFirstScene()
    {
        sceneIndex = 0;
        SceneManager.LoadScene(sceneIndex);
    }

    void Update()
    {
        if (loadingOperation == null) {
            return;
        }

        if (loadingOperation.isDone) {
            loadingOperation = null;

            GameManager.instance.SetPursuitModeOn();
        }
    }
}
