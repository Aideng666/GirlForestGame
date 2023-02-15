using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MEC;

public class AI_Mushroom_RunAway : AI_BaseClass
{
    //[SerializeField] float distanceMultiplier = 2f;
    //[SerializeField] float escapeTime = 1f;
    //[SerializeField] float timeToChangeStates = 2f;
    //Vector3 vectorFromPlayer;

    [SerializeField] float safeDashDistance = 8;
    [SerializeField] float runAwayDuration = 1;

    //float speedTemp = 0;
    //float clock = 0f;

    //Vector3 destination;

    Ray dashRay;
    bool dashStarted;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        //speedTemp = agent.speed;
        //agent.speed = 20;

        //Written this way to ignored the y axis
        //vectorFromPlayer = Vector3.Normalize(Vector3.Scale(animator.transform.position - PlayerController.Instance.transform.position, Vector3.one - Vector3.up));
        //destination = animator.transform.position + (vectorFromPlayer * distanceMultiplier);

        //RaycastHit hit;
        //if (Physics.Raycast(animator.transform.position, vectorFromPlayer, out hit, Vector3.Magnitude(destination), ~(1 << 9|1 << 10))) 
        //{
        //    //needed if we are moving the agent around without help of the navmesh
        //    //Set destination to random location in the level
        //    Debug.Log(hit.collider.gameObject);
        //    if (hit.collider.CompareTag("Environment"))
        //    {
        //        Debug.Log("Wall");
        //        Vector3 newLocation = Vector3.zero;
        //        //TODO: NOT MAKE THIS 0, 0
        //        //agent.SetDestination(Vector3.zero);
        //        agent.updatePosition = false;
        //        animator.gameObject.transform.parent.DOMove(newLocation, escapeTime);
        //    }
        //}
        //agent.SetDestination(destination);

        Vector3 directionVector = animator.transform.position - player.transform.position;
        directionVector.y = 0;
        directionVector = directionVector.normalized;

        dashRay = new Ray(new Vector3(player.transform.position.x, 0, player.transform.position.z), directionVector);
        dashStarted = false;

        agent.updateRotation = true;
        agent.destination = player.transform.position;
        //agent.speed = 0;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RaycastHit hit;

        if (Physics.Raycast(dashRay, out hit, safeDashDistance))
        {
            dashRay.direction = Quaternion.Euler(0, 45, 0) * dashRay.direction;
        }
        else if (!dashStarted)
        {
            Vector3 newPosition = player.transform.position + (dashRay.direction.normalized * safeDashDistance);
            newPosition.y = agent.transform.position.y;

            agent.transform.DOMove(newPosition, 1);

            Timing.RunCoroutine(FinishDash(animator));

            dashStarted = true;
        }

        agent.transform.LookAt(player.transform.position);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //agent.speed = speedTemp;
        agent.updatePosition = true;
    }

    IEnumerator<float> FinishDash(Animator animator)
    {
        yield return Timing.WaitForSeconds(runAwayDuration);

        animator.SetTrigger("Has_RanAway");
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    //Put a timer (wait for like 2 seconds and then go into the next state)
    //    if (clock >= timeToChangeStates) 
    //    {
    //        animator.SetTrigger("Has_RanAway");
    //    }
    //    Debug.Log(clock);
    //    clock += Time.deltaTime;
    //    // Implement code that processes and affects root motion
    //    //if (agent.enabled)
    //    //{
    //    //    //do raycast
    //    //    //do speed change
    //    //}
    //}
}
