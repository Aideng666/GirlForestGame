using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AI_SG_Throw : AI_BaseClass
{
    [SerializeField] float rotationSpeed = 0.4f;
    [SerializeField] float attackChargeTime = 1;

    float elaspedChargeTime;
    bool chargeComplete;
    bool attackFired;
    int randomPlaneChoice;

    [HideInInspector] public FMOD.Studio.EventInstance signalSFX;
    [HideInInspector] public FMOD.Studio.EventInstance throwSFX;


    //Transform projectile;
    //Vector3 defaultLocation;
    //[SerializeField] float  heightOfShot = 1f;
    //[SerializeField] float duration = 1f;
    [SerializeField] string stateChangeTrigger = "Projectile_Complete";
    //[SerializeField] GameObject target;

    private void OnEnable()
    {
        signalSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Signal");
        throwSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Golem/Throw");

    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        agent.updateRotation = false;
        agent.transform.DOLookAt(player.transform.position, rotationSpeed);

        elaspedChargeTime = 0;
        agent.speed = 0;
        attackFired = false;
        randomPlaneChoice = Random.Range(0, 2);

        //Indicate Attack
        if (randomPlaneChoice == 0)
        {
            enemyUI.IndicateAttack(Planes.Terrestrial, attackChargeTime);
            signalSFX.setParameterByName("Astral", 0);
            signalSFX.start();
        }
        else if (randomPlaneChoice == 1)
        {
            enemyUI.IndicateAttack(Planes.Astral, attackChargeTime);
            signalSFX.setParameterByName("Astral", 1);
            signalSFX.start();
        }

        //GameObject temp = Instantiate(target, player.transform.position, Quaternion.identity);

        //Getting the projectile child 
        //projectile = animator.transform.GetChild(0);
        //defaultLocation = projectile.position;

        //projectile.DOJump(PlayerController.Instance.transform.position, heightOfShot, 1, duration).OnComplete(
        //() => { animator.SetTrigger(stateChangeTrigger); projectile.position = defaultLocation; Destroy(temp); /*aoeTrigger.GetComponent<SphereCollider>().enabled = true;*/ });
    }


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (elaspedChargeTime >= attackChargeTime && !attackFired)
        {
            ThrowRock(animator);

            attackFired = true;
        }

        elaspedChargeTime += Time.deltaTime;
    }

    void ThrowRock(Animator animator)
    {
        GameObject rock = null;

        if (randomPlaneChoice == 0)
        {
            rock = ProjectilePool.Instance.GetProjectileFromPool(Planes.Terrestrial, agent.transform.position + Vector3.up, EnemyTypes.StoneGolem);
        }
        else if (randomPlaneChoice == 1)
        {
            rock = ProjectilePool.Instance.GetProjectileFromPool(Planes.Astral, agent.transform.position + Vector3.up, EnemyTypes.StoneGolem);
        }
        rock.transform.DOJump(player.transform.position, 5, 1, 1).SetEase(Ease.InCubic);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(throwSFX, agent.transform);
        throwSFX.start();
        animator.SetTrigger(stateChangeTrigger);
    }

    private void OnDestroy()
    {
        throwSFX.release();
        signalSFX.release();
    }
}
