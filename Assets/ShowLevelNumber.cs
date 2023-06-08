using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowLevelNumber : MonoBehaviour
{

    private CanvasGroup canvasGroup;
    private TextMeshProUGUI textMeshPro;

    private bool display = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnBreak += DisplayLevelText;
        GameManager.instance.OnUnbreak += HideLevelText;

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;

        textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
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
            string.Format("LEVEL {0}", GameManager.instance.level.number)
        );

        display = true;
    }

    void HideLevelText()
    {
        display = false;
    }
}
