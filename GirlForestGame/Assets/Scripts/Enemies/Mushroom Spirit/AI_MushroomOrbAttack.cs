using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;

public class AI_MushroomOrbAttack : AI_BaseClass
{
    [SerializeField] float projectileForce = 1f;
    //Used in the "3 shots". The gaps between each shot
    [SerializeField] float timeBetweenShots = 0.3f;
    [SerializeField] float rotationSpeed = 0.4f;
    [SerializeField] float attackChargeTime = 1;

    List<CoroutineHandle> handles;
    [HideInInspector] public FMOD.Studio.EventInstance shotSFX;
    [HideInInspector] public FMOD.Studio.EventInstance signalSFX;
    Animator animator;

    int randomAttackChoice;
    int randomOrbChoice;
    float elaspedChargeTime;
    bool chargeComplete;
    bool attackFired;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //This is required in order to get the cube to look at the player as it fires the orbs off
        //If this is true, unity's AI system will take over and not allow a rotation
        //It is re-enabled on exit
        agent.updateRotation = false;
        agent.transform.DOLookAt(player.transform.position, rotationSpeed);
        this.animator = animator;
        handles = new List<CoroutineHandle>();
        //handles.Add(Timing.RunCoroutine(_waitUntilLooking()));

        randomAttackChoice = Random.Range(0, 2);
        randomOrbChoice = Random.Range(0, 2);
        elaspedChargeTime = 0;
        attackFired = false;

        signalSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Signal");

        //Indicate Attack
        if (randomOrbChoice == 0)
        {
            enemyUI.IndicateAttack(Planes.Terrestrial, attackChargeTime);
            signalSFX.setParameterByName("Astral", 0);

        }
        else if (randomOrbChoice == 1)
        {
            enemyUI.IndicateAttack(Planes.Astral, attackChargeTime);
            signalSFX.setParameterByName("Astral", 1);
        }

        signalSFX.start();

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (elaspedChargeTime >= attackChargeTime && !attackFired)
        {
            if (randomAttackChoice == 0)
            {
                FanAttack();
            }
            else
            {
                handles.Add(Timing.RunCoroutine(_FiveInARow()));
            }

            attackFired = true;
        }

        elaspedChargeTime += Time.deltaTime;
    }

    void FanAttack() 
    {

        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Enemy/Fungi/Shoot", agent.gameObject);

        FireOrb((player.transform.position - agent.transform.position).normalized);
        FireOrb(Quaternion.Euler(0, 15, 0) * (player.transform.position - agent.transform.position).normalized);
        FireOrb(Quaternion.Euler(0, -15, 0) * (player.transform.position - agent.transform.position).normalized);
        FireOrb(Quaternion.Euler(0, 30, 0) * (player.transform.position - agent.transform.position).normalized);
        FireOrb(Quaternion.Euler(0, -30, 0) * (player.transform.position - agent.transform.position).normalized);

        animator.SetTrigger("Has_Fired");
    }

    IEnumerator<float> _FiveInARow() 
    {

        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(shotSFX, agent.transform);

        for (int i = 0; i < 5; i++)
        {
            FireOrb((player.transform.position - agent.transform.position).normalized);
            
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Enemy/Fungi/Shoot", agent.gameObject);

            yield return Timing.WaitForSeconds(timeBetweenShots);
        }
        //This is used so that it can delay firing the shots enough to be looking at the player, as opposed to firing any which way
        animator.SetTrigger("Has_Fired");
    }

    void FireOrb(Vector3 direction) 
    {
        if (randomOrbChoice == 0)
        {
            ProjectilePool.Instance.GetProjectileFromPool(Planes.Terrestrial, animator.transform.position, EnemyTypes.MushroomSpirit).GetComponent<Rigidbody>().AddRelativeForce(direction * projectileForce);
        }
        else
        {
            ProjectilePool.Instance.GetProjectileFromPool(Planes.Astral, animator.transform.position, EnemyTypes.MushroomSpirit).GetComponent<Rigidbody>().AddRelativeForce(direction * projectileForce);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.updateRotation = true;
        foreach (CoroutineHandle h in handles)
        {
            Timing.KillCoroutines(h);
        }
    }
}
