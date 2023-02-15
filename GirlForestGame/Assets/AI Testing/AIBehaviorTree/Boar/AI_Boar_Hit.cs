using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AI_Boar_Hit : AI_BaseClass
{
    Transform leftAxe;
    Transform rightAxe;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        leftAxe = animator.transform.GetChild(0);
        rightAxe = animator.transform.GetChild(1);

        if (enemyData.IsAttacking)
        {
            if (leftAxe.position != agent.transform.position + (agent.transform.forward + (-agent.transform.right)).normalized)
            {
                //leftAxe.DOMove(agent.transform.position + (agent.transform.forward + (-agent.transform.right)).normalized, 0.1f);
                leftAxe.position = agent.transform.position + (agent.transform.forward + (-agent.transform.right)).normalized;
            }
            if (rightAxe.position != agent.transform.position + (agent.transform.forward + agent.transform.right).normalized)
            {
                //rightAxe.DOMove(agent.transform.position + (agent.transform.forward + agent.transform.right).normalized, 0.1f);
                rightAxe.position = agent.transform.position + (agent.transform.forward + agent.transform.right).normalized;
            }

            //leftAxe.DOMove(agent.transform.position + (agent.transform.forward + (-agent.transform.right)).normalized, 0.5f);
            //rightAxe.DOMove(agent.transform.position + (agent.transform.forward + agent.transform.right).normalized, 0.5f);

            enemyData.IsAttacking = false;
        }
    }
}
