using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSpriteInRythm : MonoBehaviour
{

    private Animator animator;
    public GameObject enemy;
    
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

    public void SetExplode()
    {
        animator.SetTrigger("TriggerExplode");
        animator.SetBool("Explode", true);
    }

    public void DeActivateGameObject()
    {
        enemy.gameObject.SetActive(false);
    }

    public void SetIsGhost()
    {
        animator.SetBool("IsGhost", true);
    }

    public void SetIsNotGhost()
    {
        animator.SetBool("IsGhost", false);
    }
}
