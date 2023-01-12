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
    Transform projectile;
    BezierCurve curve;
    
    //AI_BoarEnemyClass boarParentClass;
    bool hasThrown = false;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        //sets the state in the machine so it can/can't leave the thrown state
        if (hasThrown)
        {
            animator.SetTrigger(triggerParameter);
            hasThrown = false;
        }
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //It's pretty stupid to do this eeverytime we enter the state but I don't have another way of setting this at this time
        //It's the only way that I could figure out how to throw a projectile 
        //boarParentClass =  animator.GetComponentInParent<AI_BoarEnemyClass>();
        //boarParentClass.ThrowProjectile(this);

        //Using MEC to run the coroutine
        projectile = animator.transform.GetChild(0);
        curve = animator.GetComponentInChildren<BezierCurve>();
        Timing.RunCoroutine(_projectileAnimation().CancelWith(animator.gameObject));

    }
    IEnumerator<float> _projectileAnimation()
    {
        curve.GetAnchorPoints()[2].transform.position = PlayerController.Instance.transform.position + Vector3.up;

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            projectile.position = curve.GetPointAt(time / duration);
            //transform.localRotation = Quaternion.Euler(0f, 360f * time / duration, 0f); //Spinning
            yield return Timing.WaitForOneFrame;
        }
        hasThrown = true; //The projectile has returned and can now change states back to tracking if the player has moved too far away
    }


    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    hasThrown = false;
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
