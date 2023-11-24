using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardMovement : StateMachineBehaviour
{
    [SerializeField] private float transitionSpeed;

    [SerializeField] private float animDelta;

    //private float time;
    // private bool runLastFrame;
    private void OnEnable()
    {
        animDelta = 0;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log(animator.GetBool("Run"));

        if (animator.GetBool("Run") && animDelta < 1)
        {
            animDelta += transitionSpeed * Time.deltaTime;
            //    animDelta = Mathf.Clamp(animDelta, 0, 1);
            animator.SetFloat("MoveBlend", animDelta);
            return;
        }

        if (!animator.GetBool("Run") && animDelta > 0)
        {
            animDelta -= transitionSpeed * Time.deltaTime;
            //  animDelta = Mathf.Clamp(animDelta, 0, 1);
            animator.SetFloat("MoveBlend", animDelta);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animDelta = 0;
        animator.SetFloat("MoveBlend", animDelta);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    // override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     // Implement code that processes and affects root motion
    // }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}