using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleWarningController : MonoBehaviour
{

    private Animator animator;

    private int beatsToBlink = 7;

    private SpriteRenderer spriteRenderer;

    private CanvasGroup group;

    private RectTransform rectTransform;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        group = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        Hide();
        
        if (AudioManager.instance != null) {
            AudioManager.instance.Beat += TriggerBlinkAnimationOnce;
        }
    }
    
    private void TriggerBlinkAnimationOnce()
    {
        if (group.alpha == 0) {
            return;
        }

        if (beatsToBlink == 0) {
            // spriteRenderer.enabled = false;
            // AudioManager.instance.Beat -= TriggerBlinkAnimationOnce;
            Hide();
        }

        if (beatsToBlink % 2 == 0) {
            animator.ResetTrigger("TriggerBlink");
            animator.SetTrigger("TriggerBlink");
        }

        beatsToBlink--;
    }

    public void Show()
    {
        spriteRenderer.enabled = false;
        beatsToBlink = 7;
        group.alpha = 1;
    }

    public void SetYPosition(float y)
    {
        rectTransform.localPosition = new Vector3(
            rectTransform.localPosition.x,
            y - 300,
            rectTransform.localPosition.z
        );
    }

    public void Hide()
    {
        group.alpha = 0;
        spriteRenderer.enabled = false;
    }

    void OnDestroy()
    {
        AudioManager.instance.Beat -= TriggerBlinkAnimationOnce;
    }
}
