using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AI_Mushroom_RunAway : AI_BaseClass
{
    [SerializeField] float distanceMultiplier = 2f;
    [SerializeField] float escapeTime = 1f;
    [SerializeField] float timeToChangeStates = 2f;
    Vector3 vectorFromPlayer;

    float speedTemp = 0;
    float clock = 0f;
    [HideInInspector] public FMOD.Studio.EventInstance Away= FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Fungi/Away");

    Vector3 destination;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //On Enter because once it's complete it will go to another state 
        agent = animator.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
        speedTemp = agent.speed;
        agent.speed = 20;

        //Written this way to ignored the y axis
        vectorFromPlayer = Vector3.Normalize(Vector3.Scale(animator.transform.position - PlayerController.Instance.transform.position, Vector3.one - Vector3.up));
        destination = animator.transform.position + (vectorFromPlayer * distanceMultiplier);
        RaycastHit hit;
        if (Physics.Raycast(animator.transform.position, vectorFromPlayer, out hit, Vector3.Magnitude(destination), ~(1 << 9|1 << 10))) 
        {
            //needed if we are moving the agent around without help of the navmesh
            //Set destination to random location in the level
            Debug.Log(hit.collider.gameObject);
            if (hit.collider.CompareTag("Environment"))
            {
                Debug.Log("Wall");
                Vector3 newLocation = Vector3.zero;
                //TODO: NOT MAKE THIS 0, 0
                //agent.SetDestination(Vector3.zero);
                agent.updatePosition = false;
                animator.gameObject.transform.parent.DOMove(newLocation, escapeTime);
            }
        }
        agent.SetDestination(destination);

    }

    ////OnStateUpdate is handled in the AI_BaseClass////

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.speed = speedTemp;
        agent.updatePosition = true;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Put a timer (wait for like 2 seconds and then go into the next state)
        if (clock >= timeToChangeStates) 
        {
            Away.start();
            animator.SetTrigger("Has_RanAway");
        }
        Debug.Log(clock);
        clock += Time.deltaTime;
        // Implement code that processes and affects root motion
        //if (agent.enabled)
        //{
        //    //do raycast
        //    //do speed change
        //}
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
