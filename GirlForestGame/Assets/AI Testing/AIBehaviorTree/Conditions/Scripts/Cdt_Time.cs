using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Distance_Check", menuName = "Conditions/Attack Timer")]
public class Cdt_Time : Cdt_BaseClass
{
    public float attackTime = 1f;
    public string triggerParameter = "Can_Attack";

    public override void CheckCondition(Animator animator, AI_BaseClass enemy = null)
    {
        //Checks the distance and will set the bool to true if in range, otherwise false
        if (animator.GetComponentInParent<EnemyData>().actionCooldown <= 0)
        {
            animator.SetTrigger(triggerParameter);
            animator.GetComponentInParent<EnemyData>().actionCooldown = attackTime;
        }
    }

    public override void ResetCondition(Animator animator)
    {
        animator.GetComponentInParent<EnemyData>().actionCooldown = attackTime;
    }
}