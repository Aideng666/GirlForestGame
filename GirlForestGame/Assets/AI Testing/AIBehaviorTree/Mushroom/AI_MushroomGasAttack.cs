using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_MushroomGasAttack : StateMachineBehaviour
{
    [SerializeField] GameObject gas;
    EnemyData mushroomData;
    [HideInInspector] public FMOD.Studio.EventInstance spray = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Fungi/Gas");

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //TODO: REMOVE INSTANTIATE AND USE OBJECT POOLING
        GameObject gasRef = Instantiate(gas, animator.transform.position, Quaternion.identity);

        spray.start();
        //Add force to projectile and then let it drop
        gasRef.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward);


        mushroomData = animator.GetComponentInParent<EnemyData>();
        //mushroomData.startTimer(0);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.SetBool("GasAttackReady", mushroomData.canGasAttack);
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
