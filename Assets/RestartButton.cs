using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButton : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        GameManager.instance.OnGameOver += ShowButton;
    }

    void ShowButton()
    {
        gameObject.SetActive(true);
    }
}
