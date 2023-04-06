using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Feared : AI_BaseClass
{
    [SerializeField] float fearDuration = 3;

    float startAnimSpeed;
    float startSpeed;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        startSpeed = agent.speed;
        startAnimSpeed = animator.speed;

        animator.speed = 1 / fearDuration;
        agent.speed = 3;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        Vector3 moveDir = (animator.transform.position - player.transform.position).normalized;

        agent.SetDestination(animator.transform.position + moveDir);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = startAnimSpeed;
        agent.speed = startSpeed;
    }
}
