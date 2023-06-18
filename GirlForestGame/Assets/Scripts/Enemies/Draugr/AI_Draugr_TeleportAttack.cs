using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Draugr_TeleportAttack : AI_BaseClass
{
    [SerializeField] float attackRange = 2f;

    bool attackCharged = false;
    float chargeTimer = 0;
    float totalChargeTime = 1.5f;

    LayerMask livingLayer;
    LayerMask spiritLayer;
    LayerMask defaultLayer;

    int colliderLayerMask;

    Vector3 attackPoint = Vector3.zero;

    GameObject attackIndicatorParticle;

    [HideInInspector] public FMOD.Studio.EventInstance signalSFX;
    [HideInInspector] public FMOD.Studio.EventInstance prepareSFX;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        attackCharged = false;
        chargeTimer = 0;
        attackPoint = Vector3.zero;

        livingLayer = LayerMask.NameToLayer("PlayerLiving");
        spiritLayer = LayerMask.NameToLayer("PlayerSpirit");
        defaultLayer = LayerMask.NameToLayer("Default");

        //Selects which plane to attack in
        int planeSelection = Random.Range(0, 2);

        signalSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Signal");

        if (planeSelection == (int)Planes.Terrestrial)
        {
            enemyUI.IndicateAttack(Planes.Terrestrial, totalChargeTime);
            signalSFX.setParameterByName("Astral", 0);

        }
        else if (planeSelection == (int)Planes.Astral)
        {
            enemyUI.IndicateAttack(Planes.Astral, totalChargeTime);
            signalSFX.setParameterByName("Astral", 1);
        }
        signalSFX.start();

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

        attackIndicatorParticle = ParticleManager.Instance.SpawnParticle(ParticleTypes.TeleportAttack, player.transform.position);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(prepareSFX, attackIndicatorParticle.transform);
        
        prepareSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Draugr/Prepare");
        prepareSFX.start();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (attackCharged)
        {
            //Gets the correct position to teleport to based on the attack
            Vector3 dirFromAttack = (agent.transform.position - attackPoint).normalized;
            Vector3 teleportPos = attackPoint + (dirFromAttack * attackRange);

            //Teleports and spawns the teleport particles
            ParticleManager.Instance.SpawnParticle(ParticleTypes.Teleport, agent.transform.position);
            agent.transform.position = teleportPos;

            //Checks if the attack hits the player
            Collider[] hits = Physics.OverlapSphere(agent.transform.position + (agent.transform.forward * 1.5f), attackRange, colliderLayerMask);

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
            prepareSFX.keyOff();
            //FMODUnity.RuntimeManager.AttachInstanceToGameObject(teleportSFX, agent.transform);
            //teleportSFX.start();

            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Enemy/Draugr/Teleport", agent.gameObject);

            animator.SetTrigger("AttackComplete");
        }
        else
        {

            if (chargeTimer >= totalChargeTime)
            {
                attackCharged = true;
            }
            //Dynamically moves the attack point to the player for the first half of the charge up time
            else if (chargeTimer < totalChargeTime / 2)
            {
                attackPoint = player.transform.position;

                attackIndicatorParticle.transform.position = attackPoint;
            }

            chargeTimer += Time.deltaTime;
        }
    }
    //private void OnDestroy()
    //{
    //    signalSFX.release();
    //    //teleportSFX.release();
    //    //teleportSFX.keyOff();
    //    //prepareSFX.keyOff();
    //    //prepareSFX.release();
    //}
}
