using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AI_Mushroom_RunAway : AI_BaseClass
{
    [SerializeField] float distanceMultiplier = 2f;
    [SerializeField] float escapeTime = 1f;
    Vector3 vectorFromPlayer;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //On Enter because once it's complete it will go to another state 
        agent = animator.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
        //Written this way to ignored the y axis
        vectorFromPlayer = Vector3.Normalize(Vector3.Scale(PlayerController.Instance.transform.position - animator.transform.position,Vector3.one - Vector3.up));
        Vector3 destination = animator.transform.position - (vectorFromPlayer * distanceMultiplier);

        RaycastHit hit;
        if (Physics.Raycast(animator.transform.position, vectorFromPlayer, out hit,Vector3.Magnitude(destination))) 
        {
            //Set destination to random location in the level
            if (hit.collider.CompareTag("Environment"))
            {
                Vector3 newLocation = Vector3.zero;
                //TODO NOT MAKE THIS 0, 0
                //agent.SetDestination(Vector3.zero);
                animator.gameObject.transform.DOMove(newLocation, escapeTime);
            }
        }
        agent.SetDestination(destination);
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
    //    // Implement code that processes and affects root motion
    //    //if (agent.enabled)
    //    //{
    //    //    //do raycast
    //    //    //do speed change
    //    //}
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
