using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Mushroom_Stom : AI_BaseClass
{
    [SerializeField] float distanceMultiplier = 2f;
    Vector3 vectorFromPlayer;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        vectorFromPlayer = Vector3.Normalize(animator.transform.position - PlayerController.Instance.transform.position);
    }

    ////OnStateUpdate is handled in the AI_BaseClass////

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that processes and affects root motion
        if (agent.enabled)
        {
            agent.SetDestination(animator.transform.position + (vectorFromPlayer * distanceMultiplier));
        }
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
