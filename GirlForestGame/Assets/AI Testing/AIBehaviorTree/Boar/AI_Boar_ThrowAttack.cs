using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class AI_Boar_ThrowAttack : AI_BaseClass
{
    /*
     DESCRIPTION: When the condition is triggered and it will go to the throw, it will throw it on the start and once the projectile has returned, only then will it change the state
     back to the tracking state (Hence why there are all the true false things). The condition system didn't work because this is such a niche usecase that I decided to manually trigger the 
     parameters in the animator    
     */
    public string triggerParameter = "Projectile_Has_Returned";

    [SerializeField] float duration = 1f;
    [SerializeField] float attackChargeDelay = 1;

    Transform leftAxe;
    Transform rightAxe;
    Transform defaultLeftPosition;
    Transform defaultRightPosition;
    Quaternion defaultLeftRotation;
    Quaternion defaultRightRotation;
    BezierCurve curve;

    MeshCollider leftAxeCollider;
    MeshCollider rightAxeCollider;
    TrailRenderer leftTrail;
    TrailRenderer rightTrail;

    private CoroutineHandle leftAxeThrow;
    private CoroutineHandle rightAxeThrow;

    bool projectileHasReturned = false;
    bool hasThrownProjectile = false;
    float elaspedChargetime;
    int axeSelection;
    [HideInInspector] public FMOD.Studio.EventInstance throwSFX;
    [HideInInspector] public FMOD.Studio.EventInstance catchSFX;
    //[HideInInspector] public FMOD.Studio.EventInstance flyLSFX;
    //[HideInInspector] public FMOD.Studio.EventInstance flyRSFX;
    [HideInInspector] public FMOD.Studio.EventInstance signalSFX;
    private void OnEnable()
    {
       throwSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Boar/Throw");
       catchSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Boar/Catch");
       //flyLSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Boar/Axe Fly");
       //flyRSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Boar/Axe Fly");
       signalSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Signal");

       //FMODUnity.RuntimeManager.AttachInstanceToGameObject(flyLSFX, leftAxe);
       //FMODUnity.RuntimeManager.AttachInstanceToGameObject(flyRSFX, rightAxe);

    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        leftAxe = animator.transform.GetChild(0);
        rightAxe = animator.transform.GetChild(1);
        defaultLeftPosition = animator.transform.GetChild(2);
        defaultRightPosition = animator.transform.GetChild(3);
        curve = animator.GetComponentInChildren<BezierCurve>();

        leftAxeCollider = leftAxe.GetComponent<MeshCollider>();
        rightAxeCollider = rightAxe.GetComponent<MeshCollider>();

        defaultLeftRotation = leftAxe.rotation;
        defaultRightRotation = rightAxe.rotation;
        leftTrail = leftAxe.GetComponent<TrailRenderer>();
        rightTrail = rightAxe.GetComponent<TrailRenderer>();

        projectileHasReturned = false;
        hasThrownProjectile = false;
        elaspedChargetime = 0;
        agent.speed = 0;

        axeSelection = Random.Range(0, 2);

        if (axeSelection == 0)
        {
            enemyUI.IndicateAttack(Planes.Terrestrial, attackChargeDelay);
            signalSFX.setParameterByName("Astral", 0);
            signalSFX.start();
            
        }
        else if (axeSelection == 1)
        {
            enemyUI.IndicateAttack(Planes.Astral, attackChargeDelay);
            signalSFX.setParameterByName("Astral", 1);
            signalSFX.start();
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(throwSFX, animator.gameObject.transform);

        if (elaspedChargetime >= attackChargeDelay && !hasThrownProjectile)
        {
            if (axeSelection == 0)
            {
                //Using MEC to run the coroutine
                leftAxeThrow = Timing.RunCoroutine(LeftAxeThrow().CancelWith(animator.gameObject));

                leftTrail.emitting = true;
                rightAxeCollider.enabled = false;
            }
            else if (axeSelection == 1)
            {
                rightAxeThrow = Timing.RunCoroutine(RightAxeThrow().CancelWith(animator.gameObject));

                rightTrail.emitting = true;
                leftAxeCollider.enabled = false;
            }
            
            hasThrownProjectile = true;
            enemyData.IsAttacking = true;
            throwSFX.start();
        }


        //sets the state in the machine so it can/can't leave the thrown state
        if (projectileHasReturned)
        {
            leftAxeCollider.enabled = true;
            rightAxeCollider.enabled = true;

            leftTrail.emitting = false;
            rightTrail.emitting = false;

            animator.SetTrigger(triggerParameter);
            projectileHasReturned = false;
        }

        elaspedChargetime += Time.deltaTime;
    }

    IEnumerator<float> LeftAxeThrow()
    {
        curve.GetAnchorPoints()[0].transform.position = defaultLeftPosition.position;
        curve.GetAnchorPoints()[1].transform.position = player.transform.position + Vector3.up + (Quaternion.Euler(0, 30, 0) * (agent.transform.position - player.transform.position).normalized);
        curve.GetAnchorPoints()[2].transform.position = player.transform.position + Vector3.up;
        curve.GetAnchorPoints()[3].transform.position = player.transform.position + Vector3.up + (Quaternion.Euler(0, -30, 0) * (agent.transform.position - player.transform.position).normalized);
        curve.GetAnchorPoints()[4].transform.position = defaultLeftPosition.position;

        leftAxe.rotation = Quaternion.Euler(90, 0, 0);

            
        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            leftAxe.position = curve.GetPointAt(time / duration);
            //leftAxe.rotation = Quaternion.Euler(0, leftAxe.rotation.y, 0);
            leftAxe.Rotate(Vector3.up, 3, Space.World);

            curve.GetAnchorPoints()[0].transform.position = defaultLeftPosition.position;

            yield return Timing.WaitForOneFrame;
        }

        leftAxe.rotation = defaultLeftRotation;

        projectileHasReturned = true;
        enemyData.IsAttacking = false;
    }

    IEnumerator<float> RightAxeThrow()
    {
        curve.GetAnchorPoints()[0].transform.position = defaultRightPosition.position;
        curve.GetAnchorPoints()[1].transform.position = player.transform.position + Vector3.up + (Quaternion.Euler(0, 30, 0) * (agent.transform.position - player.transform.position).normalized);
        curve.GetAnchorPoints()[2].transform.position = player.transform.position + Vector3.up;
        curve.GetAnchorPoints()[3].transform.position = player.transform.position + Vector3.up + (Quaternion.Euler(0, -30, 0) * (agent.transform.position - player.transform.position).normalized);
        curve.GetAnchorPoints()[4].transform.position = defaultRightPosition.position;

        rightAxe.rotation = Quaternion.Euler(90, 0, 0);

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            rightAxe.position = curve.GetPointAt(time / duration);
            //rightAxe.rotation = Quaternion.Euler(0, rightAxe.rotation.y, 0);
            rightAxe.Rotate(Vector3.up, 3, Space.World);

            curve.GetAnchorPoints()[0].transform.position = defaultRightPosition.position;

            yield return Timing.WaitForOneFrame;
        }

        rightAxe.rotation = defaultRightRotation;

        projectileHasReturned = true;
        enemyData.IsAttacking = false;
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var cond in conditions)
        {
            cond.ResetCondition(animator);
        }

        //This is a safety net in case something malfunctions with MEC
        Timing.KillCoroutines(leftAxeThrow);
        Timing.KillCoroutines(rightAxeThrow);
    }
}
