using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;


public class AI_Boar_DashAttack : AI_BaseClass 
{
    [SerializeField] string triggerParameter = "Dash_Attack_Completed";
    [SerializeField] float duration = 3;
    [SerializeField] float attackRange = 1.5f;

    bool attackCharged = false;
    float attackTimer = 0;
    float dashTimer = 0;
    float chargeTimer = 0;
    float timeBetweenEachAttack = 0.33f;
    float totalChargeTime = 1;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        dashTimer = 0;
        attackTimer = 0;
        agent.speed = 0;

        attackCharged = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (dashTimer >= duration)
        {
            animator.SetTrigger(triggerParameter);
        }

        if (attackCharged)
        {
            agent.speed = enemyData.enemyMaxSpeed;
            agent.SetDestination(player.transform.position);
            
            //Change this collision part into a function when we get animations
            //Also swap between Terrestial and Astral
            if (attackTimer >= timeBetweenEachAttack)
            {
                Collider[] hits = Physics.OverlapSphere(agent.transform.position + (agent.transform.forward * 1.5f), attackRange);

                if (hits.Length > 0)
                {
                    foreach (Collider hit in hits)
                    {
                        if (hit.TryGetComponent(out PlayerCombat playerHit))
                        {
                            //player.playerCombat.ApplyKnockback(agent.transform.forward, 5);
                            playerHit.TakeDamage();
                        }
                    }
                }

                attackTimer = 0;
            }

            attackTimer += Time.deltaTime;
            dashTimer += Time.deltaTime;
        }
        else
        {
            if (chargeTimer >= totalChargeTime)
            {
                attackCharged = true;
            }

            chargeTimer += Time.deltaTime;
        }
    }
}
