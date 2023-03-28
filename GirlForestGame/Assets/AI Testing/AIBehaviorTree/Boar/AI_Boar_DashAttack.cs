using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;


public class AI_Boar_DashAttack : AI_BaseClass 
{
    [SerializeField] string triggerParameter = "Dash_Attack_Completed";
    [SerializeField] float duration = 3;

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
        OinkSFX.release();
        ChargeSFX.start();
        attackCharged = false;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(ChargeSFX, animator.gameObject.transform);

        if (dashTimer >= duration)
        {
            //ChargeSFX.keyOff();
            ChargeSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            animator.SetTrigger(triggerParameter);
            
        }

        if (attackCharged)
        {
            agent.speed = animator.GetComponentInParent<EnemyData>().enemyMaxSpeed;
            agent.SetDestination(PlayerController.Instance.transform.position);

            //Change this collision part into a function when we get animations
            //Also swap between Terrestial and Astral
            if (attackTimer >= timeBetweenEachAttack)
            {
                Collider[] hits = Physics.OverlapSphere(agent.transform.position + (agent.transform.forward * 1.5f), 1.5f);

                if (hits.Length > 0)
                {
                    foreach (Collider hit in hits)
                    {
                        if (hit.TryGetComponent(out PlayerController player))
                        {
                            //player.playerCombat.ApplyKnockback(agent.transform.forward, 5);
                            player.playerCombat.TakeDamage();
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
        ChargeSFX.keyOff();
        ChargeSFX.release();
    }
}
