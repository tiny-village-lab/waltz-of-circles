using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPursuitSpriteController : MonoBehaviour
{

    private Animator animator;
    public GameObject enemy;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /*
    Triggers animation of explosion. The animator will 
    call then The method to destroy the parent object
    */
    public void Explode()
    {
        animator.SetTrigger("DestroyTrigger");
    }

    public void ExplodeAnimationFinished()
    {
        Destroy(enemy.gameObject);
    }
}
