using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSpriteInRythm : MonoBehaviour
{

    private Animator animator;
    
    private void TriggerIdleAnimationOnce()
    {
        animator.ResetTrigger("TriggerWobble");
        animator.SetTrigger("TriggerWobble");
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
        if (AudioManager.instance != null) {
            AudioManager.instance.Beat += TriggerIdleAnimationOnce;
        }
    }

    private void OnDestroy()
    {
        if (AudioManager.instance != null) {
            AudioManager.instance.Beat -= TriggerIdleAnimationOnce;
        }
    }

    public void IsUntouchable()
    {
        animator.SetBool("IsUntouchable", true);
    }

    public void IsTouchable()
    {
        animator.SetBool("IsUntouchable", false);
    }

    public void SetIsDead()
    {
        animator.SetBool("IsDead", true);
    }
}
