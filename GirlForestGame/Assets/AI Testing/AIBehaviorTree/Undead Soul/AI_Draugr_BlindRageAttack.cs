using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Draugr_BlindRageAttack : AI_BaseClass
{
    [SerializeField] float duration = 3;
    [SerializeField] float attackRange = 1.5f;

    bool attackCharged = false;
    float chargeTimer = 0;
    float totalChargeTime = 1;
    float attackTimer = 0;
    float elaspedAttackTime = 0;
    float timeBetweenEachAttack = 0.33f;

    LayerMask livingLayer;
    LayerMask spiritLayer;
    LayerMask defaultLayer;

    int colliderLayerMask;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        attackTimer = 0;
        agent.speed = 0;
        elaspedAttackTime = 0;

        attackCharged = false;

        livingLayer = LayerMask.NameToLayer("PlayerLiving");
        spiritLayer = LayerMask.NameToLayer("PlayerSpirit");
        defaultLayer = LayerMask.NameToLayer("Default");

        int planeSelection = Random.Range(0, 2);

        if (planeSelection == (int)Planes.Terrestrial)
        {
            enemyUI.IndicateAttack(Planes.Terrestrial, totalChargeTime);
        }
        else if (planeSelection == (int)Planes.Astral)
        {
            enemyUI.IndicateAttack(Planes.Astral, totalChargeTime);
        }

        //Creates the correct layer mask for the colliders to hit the proper enemies at any given time
        colliderLayerMask = (1 << defaultLayer);

        if (planeSelection == (int)Planes.Terrestrial)
        {
            colliderLayerMask |= (1 << livingLayer);
            colliderLayerMask &= ~(1 << spiritLayer);
        }
        else
        {
            colliderLayerMask |= (1 << spiritLayer);
            colliderLayerMask &= ~(1 << livingLayer);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (elaspedAttackTime >= duration)
        {
            animator.SetTrigger("AttackComplete");
        }

        if (attackCharged)
        {
            if (attackTimer >= timeBetweenEachAttack)
            {
                Collider[] hits = Physics.OverlapSphere(agent.transform.position + (agent.transform.forward * 1.5f), attackRange, colliderLayerMask);

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
        }
        else
        {
            if (chargeTimer >= totalChargeTime)
            {
                attackCharged = true;
            }

            chargeTimer += Time.deltaTime;
        }

        elaspedAttackTime += Time.deltaTime;
    }
}
