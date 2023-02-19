using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Winded : AI_BaseClass
{
    float duration;
    float startSpeed;
    float startAnimSpeed;
    //float elaspedTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        duration = PlayerController.Instance.playerMarkings.GetWindedDuration();

        startSpeed = agent.speed;
        startAnimSpeed = animator.speed;

        animator.speed = 1 / duration;
        agent.speed = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.speed = startSpeed;
        animator.speed = startAnimSpeed;
    }
}
