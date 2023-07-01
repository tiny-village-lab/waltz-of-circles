using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleWarningController : MonoBehaviour
{

    private Animator animator;

    private int beatsToBlink = 7;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (AudioManager.instance != null) {
            AudioManager.instance.Beat += TriggerBlinkAnimationOnce;
        }
    }
    
    private void TriggerBlinkAnimationOnce()
    {
        if (beatsToBlink == 0) {
            spriteRenderer.enabled = false;
            AudioManager.instance.Beat -= TriggerBlinkAnimationOnce;
        }

        if (beatsToBlink % 2 == 0) {
            animator.ResetTrigger("TriggerBlink");
            animator.SetTrigger("TriggerBlink");
        }

        beatsToBlink--;
    }

    void OnDestroy()
    {
        AudioManager.instance.Beat -= TriggerBlinkAnimationOnce;
    }
}
