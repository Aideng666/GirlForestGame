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
    BezierCurve curve;

    SphereCollider leftAxeCollider;
    SphereCollider rightAxeCollider;

    private CoroutineHandle leftAxeThrow;
    private CoroutineHandle rightAxeThrow;

    bool projectileHasReturned = false;
    bool hasThrownProjectile = false;
    float elaspedChargetime;
    int axeSelection;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        leftAxe = animator.transform.GetChild(0);
        rightAxe = animator.transform.GetChild(1);
        defaultLeftPosition = animator.transform.GetChild(2);
        defaultRightPosition = animator.transform.GetChild(3);
        curve = animator.GetComponentInChildren<BezierCurve>();

        leftAxeCollider = leftAxe.GetComponent<SphereCollider>();
        rightAxeCollider = rightAxe.GetComponent<SphereCollider>();

        projectileHasReturned = false;
        hasThrownProjectile = false;
        elaspedChargetime = 0;
        agent.speed = 0;

        axeSelection = Random.Range(0, 2);

        if (axeSelection == 0)
        {
            enemyUI.IndicateAttack(Planes.Terrestrial, attackChargeDelay);
        }
        else if (axeSelection == 1)
        {
            enemyUI.IndicateAttack(Planes.Astral, attackChargeDelay);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if (elaspedChargetime >= attackChargeDelay && !hasThrownProjectile)
        {
            if (axeSelection == 0)
            {
                //Using MEC to run the coroutine
                leftAxeThrow = Timing.RunCoroutine(LeftAxeThrow().CancelWith(animator.gameObject));

                rightAxeCollider.enabled = false;
            }
            else if (axeSelection == 1)
            {
                rightAxeThrow = Timing.RunCoroutine(RightAxeThrow().CancelWith(animator.gameObject));

                leftAxeCollider.enabled = false;
            }

            hasThrownProjectile = true;
            enemyData.IsAttacking = true;
        }


        //sets the state in the machine so it can/can't leave the thrown state
        if (projectileHasReturned)
        {
            leftAxeCollider.enabled = true;
            rightAxeCollider.enabled = true;

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

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            leftAxe.position = curve.GetPointAt(time / duration);

            curve.GetAnchorPoints()[0].transform.position = defaultLeftPosition.position;

            yield return Timing.WaitForOneFrame;
        }

        projectileHasReturned = true;
        enemyData.IsAttacking = false;
    }

    IEnumerator<float> RightAxeThrow()
    {
        curve.GetAnchorPoints()[0].transform.position = defaultRightPosition.position;
        curve.GetAnchorPoints()[1].transform.position = player.transform.position + Vector3.up + (Quaternion.Euler(0, 30, 0) * (agent.transform.position - player.transform.position).normalized);
        curve.GetAnchorPoints()[2].transform.position = player.transform.position + Vector3.up;
        curve.GetAnchorPoints()[3].transform.position = player.transform.position + Vector3.up + (Quaternion.Euler(0, -30, 0) * (agent.transform.position - player.transform.position).normalized);

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            rightAxe.position = curve.GetPointAt(time / duration);

            curve.GetAnchorPoints()[0].transform.position = defaultRightPosition.position;

            yield return Timing.WaitForOneFrame;
        }

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
