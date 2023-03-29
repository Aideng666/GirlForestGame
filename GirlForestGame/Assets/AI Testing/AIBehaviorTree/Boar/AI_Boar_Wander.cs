using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;

/// <summary>
/// Attach this script to the state that will track the player. It will update the destination of the AI every frame to the player's 
/// </summary>
public class AI_Boar_Wander : AI_BaseClass
{
    Transform leftAxe;
    Transform rightAxe;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        leftAxe = animator.transform.GetChild(0);
        rightAxe = animator.transform.GetChild(1);
        agent.speed = 4;

        //if (leftAxe.position != agent.transform.position + (agent.transform.forward + (-agent.transform.right)).normalized)
        //{
        //    leftAxe.DOMove(agent.transform.position + (agent.transform.forward + (-agent.transform.right)).normalized, 0.1f);
        //}
        //if (rightAxe.position != agent.transform.position + (agent.transform.forward + agent.transform.right).normalized)
        //{
        //    rightAxe.DOMove(agent.transform.position + (agent.transform.forward + agent.transform.right).normalized, 0.1f);
        //}
    }

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
            agent.SetDestination(player.transform.position);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (leftAxe.position != agent.transform.position + (agent.transform.forward + (-agent.transform.right)).normalized)
        {
            leftAxe.position = agent.transform.position + (agent.transform.forward + (-agent.transform.right)).normalized;
        }
        if (rightAxe.position != agent.transform.position + (agent.transform.forward + agent.transform.right).normalized)
        {
            rightAxe.position = agent.transform.position + (agent.transform.forward + agent.transform.right).normalized;
        }

        enemyData.RunCooldownTimer();
    }
}
