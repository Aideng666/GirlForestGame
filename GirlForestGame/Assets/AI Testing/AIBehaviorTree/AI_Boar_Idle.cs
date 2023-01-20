using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AI_Boar_Idle : AI_BaseClass
{
    [SerializeField] float idleTimer = 2;
    [SerializeField] string triggerParameter = "Idle_Done";
    float elaspedTime;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        agent.speed = 0;
        elaspedTime = 0;
    }

    ////OnStateUpdate is handled in the AI_BaseClass////

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        elaspedTime += Time.deltaTime;

        if (elaspedTime >= idleTimer)
        {
            animator.SetTrigger(triggerParameter);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
