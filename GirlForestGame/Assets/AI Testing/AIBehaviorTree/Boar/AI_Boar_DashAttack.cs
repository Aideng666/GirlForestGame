using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;


public class AI_Boar_DashAttack : AI_BaseClass 
{
    [SerializeField] string triggerParameter = "Dash_Attack_Completed";
    [SerializeField] float duration = 3;
    float elaspedTime = 0;

    bool attackCharged = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        elaspedTime = 0;
        agent.speed = 0;

        Timing.RunCoroutine(BeginAttackCharge());
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (elaspedTime >= duration)
        {
            animator.SetTrigger(triggerParameter);
        }

        if (attackCharged)
        {
            agent.speed = animator.GetComponentInParent<EnemyData>().enemyMaxSpeed;
            agent.SetDestination(PlayerController.Instance.transform.position);
            //Attack over and over again, can do this using animation events when we have the anims

            elaspedTime += Time.deltaTime;
        }
    }

    IEnumerator<float> BeginAttackCharge()
    {
        attackCharged = false;
        float elaspedChargeTime = 0;
        float totalChargeTime = 1f;

        while (!attackCharged)
        {
            elaspedChargeTime += Time.deltaTime;

            Debug.Log("Paused");

            if (elaspedChargeTime >= totalChargeTime)
            {
                attackCharged = true;
            }

            yield return 0f;
        }

        yield return 0f;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("Attack");
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
