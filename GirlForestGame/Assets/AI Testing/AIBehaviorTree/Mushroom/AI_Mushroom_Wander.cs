using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class AI_Mushroom_Wander : AI_BaseClass
{
    [SerializeField] Vector2 randomTimingRange;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
        Timing.RunCoroutine(_moveOnTimer(animator).CancelWith(animator.gameObject));
    }

    IEnumerator<float> _moveOnTimer(Animator animator) 
    {
        bool loop = true;
        while (loop)
        {
            //Pick Random destination in room, move at low speed
            agent.SetDestination(animator.transform.position + new Vector3(Random.Range(-4, 4), 0, Random.Range(-4, 4)));

            //Wait before moving again
            yield return Timing.WaitForSeconds(Random.Range(randomTimingRange.x, randomTimingRange.y));
        }
    }

    ////OnStateUpdate is handled in the AI_BaseClass////

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
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
