using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Distance_Check", menuName = "Conditions/Attack Timer")]
public class Cdt_Time : Cdt_BaseClass
{
    public float attackTime = 1f;
    public string boolParameter = "Can_Attack";

    public override void CheckCondition(Animator animator, AI_BaseClass enemy = null)
    {
        //If the action cooldown is less than 0 then the attack Cooldown is complete.
        if (animator.GetComponentInParent<EnemyData>().actionCooldown <= 0)
        {
            animator.SetTrigger(boolParameter);
            animator.GetComponentInParent<EnemyData>().actionCooldown = attackTime;
        }
    }

    public override void ResetCondition(Animator animator)
    {
        animator.GetComponentInParent<EnemyData>().actionCooldown = attackTime;
    }
}