using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using DG.Tweening;

public class AI_Mushroom_Wander : AI_BaseClass
{
    [SerializeField] Vector2 randomTimingRange;
    [SerializeField] float moveDistance;

    float elaspedTime;
    float moveTimer;

    Ray moveRay;

    //private CoroutineHandle moveCoroutine;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        elaspedTime = 0;
        moveTimer = Random.Range(randomTimingRange.x, randomTimingRange.y);
        agent.updateRotation = false;

        animator.SetBool("Close_To_Player", false);

        moveRay = new Ray(agent.transform.position, new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized);

        //moveCoroutine = Timing.RunCoroutine(_moveOnTimer(animator).CancelWith(animator.gameObject));
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        animator.GetComponentInParent<EnemyData>().RunCooldownTimer();

        if (elaspedTime >= moveTimer)
        {
            if (Physics.Raycast(moveRay, moveDistance * 3))
            {
                moveRay.direction = Quaternion.Euler(0, 90, 0) * moveRay.direction;
            }
            else
            {
                agent.transform.DOMove(animator.transform.position + (moveRay.direction * Random.Range(moveDistance / 2, moveDistance)), 0.6f).SetEase(Ease.InOutCubic);

                elaspedTime = -0.6f;
                moveTimer = Random.Range(randomTimingRange.x, randomTimingRange.y);

                moveRay = new Ray(agent.transform.position, new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized);
            }
        }

        agent.transform.LookAt(player.transform.position);
        elaspedTime += Time.deltaTime;
    }

    //IEnumerator<float> _moveOnTimer(Animator animator)
    //{
    //    while (true)
    //    {
    //        //Wait before moving again
    //        yield return Timing.WaitForSeconds(Random.Range(randomTimingRange.x, randomTimingRange.y));
    //        //Pick Random destination in room, move at low speed
    //        agent.SetDestination(animator.transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2)));
    //    }
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Doing this to make sure that the coroutine stops when it leaves the state
        //It may be redundant but it's a safety net because we aren't destroying the object when changing states
        //Timing.KillCoroutines(moveCoroutine);

        //Stops all the movement to favor any other state
        agent.ResetPath();
    }
}
