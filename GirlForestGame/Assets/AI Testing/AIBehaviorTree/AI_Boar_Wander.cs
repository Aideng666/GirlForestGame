using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this script to the state that will track the player. It will update the destination of the AI every frame to the player's 
/// </summary>
public class AI_Boar_Wander : AI_BaseClass
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
        agent.enabled = true;
        agent.speed = 2;
        //turns AI movement on
    }

    ////OnStateUpdate is handled in the AI_BaseClass////

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Turns off the AI movement
        agent.enabled = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Targets the player
        if (agent.enabled)
        {
            agent.SetDestination(PlayerController.Instance.transform.position);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        animator.GetComponentInParent<EnemyData>().RunCooldownTimer();
    }
}
