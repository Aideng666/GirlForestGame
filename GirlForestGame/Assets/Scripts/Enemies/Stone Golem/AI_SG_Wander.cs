using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_SG_Wander : AI_BaseClass
{
    [SerializeField] Vector2 randomTimingRange;
    [SerializeField] float moveDistance;

    float elaspedTime;
    float moveTimer;

    bool isMoving = false;

    Vector3 previousPos;
    Ray moveRay;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        agent.speed = 1;

        elaspedTime = 0;
        moveTimer = Random.Range(randomTimingRange.x, randomTimingRange.y);

        moveRay = new Ray(agent.transform.position, (Vector3.zero - agent.transform.position).normalized);
        moveRay.direction = Quaternion.Euler(0, Random.Range(-45, 45), 0) * moveRay.direction;
    }
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        animator.GetComponentInParent<EnemyData>().RunCooldownTimer();

        //Moves the golem on a timer
        if (agent.enabled)
        {
            if (isMoving)
            {
                if (Vector3.Distance(previousPos, agent.transform.position) >= moveDistance)
                {
                    isMoving = false;

                    elaspedTime = 0f;
                    moveTimer = Random.Range(randomTimingRange.x, randomTimingRange.y);

                    moveRay = new Ray(agent.transform.position, (Vector3.zero - agent.transform.position).normalized);
                    moveRay.direction = Quaternion.Euler(0, Random.Range(-45, 45), 0) * moveRay.direction;

                    agent.speed = 0;
                    agent.updateRotation = false;
                }
            }
            else if (elaspedTime >= moveTimer)
            {
                if (Physics.Raycast(moveRay, moveDistance * 2, LayerMask.NameToLayer("Default")))
                {
                    moveRay.direction = Quaternion.Euler(0, 20, 0) * moveRay.direction;
                }
                else
                {
                    isMoving = true;

                    previousPos = agent.transform.position;
                    agent.destination = agent.transform.position + (moveRay.direction * moveDistance * 2);
                    agent.speed = 1;
                    agent.updateRotation = true;
                }
            }
            else
            {
                elaspedTime += Time.deltaTime;
            }
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    agent.ResetPath();
    //}
}
