using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NextHeartCountdown : MonoBehaviour
{

    private TextMeshProUGUI textMeshPro;
    private CanvasGroup canvasGroup;

    private bool isFadingIn = false;

    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    void Update()
    {
        if (isFadingIn == false) {
            StopCoroutine(Blink());
        }
    }

    public void UpdateCountdownText(int count)
    {
        if (count < 1) {
            canvasGroup.alpha = 0;
        }
        
        textMeshPro.SetText(
            string.Format("{0}", count)
        );

        if (count > 0) {
            StartCoroutine(Blink());
        }
    }

    IEnumerator Blink()
    {
        isFadingIn = true;

        for (float a=0f; a<0.8f; a+=0.05f){
            canvasGroup.alpha = a;
            yield return null;
        }

        isFadingIn = false;
    }
}
