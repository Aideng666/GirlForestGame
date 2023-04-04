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

    [HideInInspector] public FMOD.Studio.EventInstance ChargeSFX;
    [HideInInspector] public FMOD.Studio.EventInstance OinkSFX;  

    private void OnEnable()
    {
        ChargeSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Boar/Dash");
        OinkSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Boar/Oink");
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(OinkSFX, animator.gameObject.transform);

        dashTimer = 0;
        attackTimer = 0;
        agent.speed = 0;
        OinkSFX.start();
        ChargeSFX.start();
        attackCharged = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(ChargeSFX, animator.gameObject.transform);
        
        if (dashTimer >= duration)
        {
            ChargeSFX.keyOff();
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
    private void OnDestroy()
    {
        OinkSFX.release();
        ChargeSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        ChargeSFX.release();
    }
}
