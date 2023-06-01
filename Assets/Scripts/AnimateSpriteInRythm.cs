using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSpriteInRythm : MonoBehaviour
{

    [SerializeField] private Heartbeat heartbeat;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    private void TriggerIdleAnimationOnce()
    {
        animator.ResetTrigger("TriggerWobble");
        animator.SetTrigger("TriggerWobble");
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        if (heartbeat != null) {
            heartbeat.Tick += TriggerIdleAnimationOnce;
        }
    }

    private void OnDestroy()
    {

        if (heartbeat != null) {
            heartbeat.Tick -= TriggerIdleAnimationOnce;
        }
    }
}
