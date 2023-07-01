using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowLevelNumber : MonoBehaviour
{

    private CanvasGroup canvasGroup;
    private TextMeshProUGUI textMeshPro;

    private bool display = false;

    private bool displayLock = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnBreak += DisplayLevelText;
        GameManager.instance.OnUnbreak += HideLevelText;
        GameManager.instance.OnGameOver += DisplayGameOver;

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (displayLock) {
            canvasGroup.alpha = 1.0f;
            return;
        }

        if (display && canvasGroup.alpha < 1) {
            canvasGroup.alpha += 10.0f * Time.unscaledDeltaTime;
        }

        if (!display && canvasGroup.alpha > 0) {
            canvasGroup.alpha -= 3.0f * Time.unscaledDeltaTime;
        }
    }

    void DisplayLevelText()
    {
        textMeshPro.SetText(
            string.Format("LEVEL {0}", LevelManager.instance.GetCurrentLevel().number)
        );

        display = true;
    }

    void DisplayGameOver()
    {
        print("here");
        textMeshPro.SetText(
            string.Format("GAME OVER")
        );

        display = true;
        displayLock = true;
    }

    void HideLevelText()
    {
        display = false;
    }
}
