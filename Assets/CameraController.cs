using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Animator animator;

    private bool isShaky = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isShaky != GameManager.instance.IsOnPursuitMode()) {
            isShaky = GameManager.instance.IsOnPursuitMode();

            animator.SetBool("IsShaky", isShaky);
        }     
    }
}
