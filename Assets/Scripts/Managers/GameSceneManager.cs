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

    public void LoadNextScene()
    {
        sceneIndex++;
        loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
    }

    void Update()
    {
        if (loadingOperation == null) {
            return;
        }

        if (loadingOperation.isDone) {
            GameManager.instance.ZoomOut();
            loadingOperation = null;

            GameManager.instance.SetPursuitModeOn();
        }
    }
}
