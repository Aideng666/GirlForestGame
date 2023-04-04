using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_SG_Stomp : AI_BaseClass
{
    [SerializeField] float attackIndicationTime = 0.5f;
    [SerializeField] float stompRadius = 2;

    int randomPlaneChoice;

    LayerMask livingLayer;
    LayerMask spiritLayer;
    LayerMask defaultLayer;

    float elaspedTime;
    int stompCount;

    [HideInInspector] public FMOD.Studio.EventInstance stompSFX;
    [HideInInspector] public FMOD.Studio.EventInstance signalSFX;

    private void OnEnable()
    {
        stompSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Golem/Stomp");
        signalSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Signal");



    }
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        agent.speed = 0;
        elaspedTime = 0;
        stompCount = 0;
        randomPlaneChoice = Random.Range(0, 2);

        livingLayer = LayerMask.NameToLayer("PlayerLiving");
        spiritLayer = LayerMask.NameToLayer("PlayerSpirit");
        defaultLayer = LayerMask.NameToLayer("Default");

        //Indicate Attack
        if (randomPlaneChoice == 0)
        {
            enemyUI.IndicateAttack(Planes.Terrestrial, attackIndicationTime);
            signalSFX.setParameterByName("Astral", 0);
            signalSFX.start();
        }
        else if (randomPlaneChoice == 1)
        {
            enemyUI.IndicateAttack(Planes.Astral, attackIndicationTime);
            signalSFX.setParameterByName("Astral", 1);
            signalSFX.start();
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (stompCount >= 2)
        {
            animator.SetTrigger("Stomp_Complete");
            stompSFX.keyOff();
        }
        else if (elaspedTime >= 0.5f)
        {
            Stomp();

            elaspedTime = 0;
        }

        elaspedTime += Time.deltaTime;
    }

    //Use in animation event to time the stomp to the animation
    public void Stomp()
    {
        //Creates the correct layer mask for the colliders to hit the proper enemies at any given time
        int colliderLayerMask = (1 << defaultLayer);

        if (randomPlaneChoice == 0)
        {
            colliderLayerMask |= (1 << livingLayer);
            colliderLayerMask &= ~(1 << spiritLayer);

            ParticleManager.Instance.SpawnParticle(ParticleTypes.GolemStomp, agent.transform.position);

            Collider[] collidersHit = Physics.OverlapSphere(agent.transform.position, stompRadius, colliderLayerMask);

            foreach (Collider collider in collidersHit)
            {
                if (collider.gameObject.CompareTag("Player"))
                {
                    player.playerCombat.TakeDamage();
                    //player.playerCombat.ApplyKnockback((player.transform.position - agent.transform.position).normalized, 4);
                }
            }
        }
        else if (randomPlaneChoice == 1)
        {
            colliderLayerMask |= (1 << spiritLayer);
            colliderLayerMask &= ~(1 << livingLayer);

            ParticleManager.Instance.SpawnParticle(ParticleTypes.GolemStomp, agent.transform.position);

            Collider[] collidersHit = Physics.OverlapSphere(agent.transform.position, stompRadius, colliderLayerMask);

            foreach (Collider collider in collidersHit)
            {
                if (collider.gameObject.CompareTag("Player"))
                {
                    player.playerCombat.TakeDamage();
                    //player.playerCombat.ApplyKnockback((player.transform.position - agent.transform.position).normalized, 4);
                }
            }
        }
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(stompSFX, agent.transform);
        stompSFX.start();
        stompCount++;
    }

    private void OnDestroy()
    {
        stompSFX.release();
    }
}
