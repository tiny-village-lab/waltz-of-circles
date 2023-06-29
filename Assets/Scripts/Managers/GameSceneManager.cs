using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{

    public static GameSceneManager instance;

    private int sceneIndex = 0;

    void Awake()
    {
        if (!instance) {
            instance = this;
        }
    }

    public void LoadNextScene()
    {
        sceneIndex++;
        SceneManager.LoadSceneAsync(sceneIndex);
        GameManager.instance.ZoomOut();
    }
}
