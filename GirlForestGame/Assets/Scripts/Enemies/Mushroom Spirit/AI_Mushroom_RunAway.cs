using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MEC;
using UnityEngine.SceneManagement;

public class AI_Mushroom_RunAway : AI_BaseClass
{
    //[SerializeField] float distanceMultiplier = 2f;
    //[SerializeField] float escapeTime = 1f;
    //[SerializeField] float timeToChangeStates = 2f;
    //Vector3 vectorFromPlayer;

    [SerializeField] float safeDashDistance = 8;
    [SerializeField] float runAwayDuration = 1;
    [HideInInspector] public FMOD.Studio.EventInstance Away;

    //float speedTemp = 0;
    //float clock = 0f;

    //Vector3 destination;

    Ray dashRay;
    bool dashStarted;

    private void OnEnable()
    {
        Away = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Fungi/Dash");

    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial"))
        {
            TutorialManager.Instance.TriggerTutorialSection(2, true);
        }

        Vector3 directionVector = animator.transform.position - player.transform.position;
        directionVector.y = 0;
        directionVector = directionVector.normalized;

        dashRay = new Ray(new Vector3(player.transform.position.x, 0, player.transform.position.z), directionVector);
        dashStarted = false;

        agent.updateRotation = true;
        agent.destination = player.transform.position;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RaycastHit hit;
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Away, agent.transform);

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
            
            Away.start();
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

    private void OnDestroy()
    {
        Away.release();
    }
}
