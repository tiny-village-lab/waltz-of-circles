using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{

    private TextMeshProUGUI textMeshProUGUI;

    // Start is called before the first frame update
    void Start()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        textMeshProUGUI.SetText(
            string.Format("SCORE {0}", GameManager.instance.GetScore().ToString("D4"))
        );
    }
}
