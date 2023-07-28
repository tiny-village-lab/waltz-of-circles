using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowWaveNumber : MonoBehaviour
{

    private CanvasGroup canvasGroup;
    private TextMeshProUGUI textMeshPro;

    private bool display = false;

    private bool displayLock = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnBreak += DisplayWaveText;
        GameManager.instance.OnUnbreak += HideWaveText;
        GameManager.instance.OnGameOver += DisplayGameOver;
        GameManager.instance.OnRestart += Init;

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    void Init()
    {
        canvasGroup.alpha = 0;
        displayLock = false;
        HideWaveText();
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

    void DisplayWaveText()
    {
        textMeshPro.SetText(
            string.Format("WAVE {0}", WaveManager.instance.GetCurrentWave().number)
        );

        display = true;
    }

    void DisplayGameOver()
    {
        textMeshPro.SetText(
            string.Format("GAME OVER")
        );

        display = true;
        displayLock = true;
    }

    void HideWaveText()
    {
        display = false;
    }
}
