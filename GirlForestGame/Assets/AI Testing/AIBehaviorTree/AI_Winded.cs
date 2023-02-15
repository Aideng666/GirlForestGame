using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Winded : AI_BaseClass
{
    float duration;
    float elaspedTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        duration = PlayerController.Instance.playerMarkings.GetWindedDuration();
        elaspedTime = 0;

        agent.speed = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (elaspedTime >= duration)
        {
            animator.SetBool("Winded", false);
        }

        elaspedTime += Time.deltaTime;
    }
}
