using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class AI_Mushroom_Wander : AI_BaseClass
{
    [SerializeField] Vector2 randomTimingRange;

    private CoroutineHandle moveTimer;
    //private AI_MushroomData mushroomData;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //mushroomData = animator.GetComponentInParent<AI_MushroomData>();
        agent = animator.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
        moveTimer = Timing.RunCoroutine(_moveOnTimer(animator).CancelWith(animator.gameObject));

    }

    IEnumerator<float> _moveOnTimer(Animator animator) 
    {
        while (true)
        {
            //Wait before moving again
            yield return Timing.WaitForSeconds(Random.Range(randomTimingRange.x, randomTimingRange.y));
            //Pick Random destination in room, move at low speed
            agent.SetDestination(animator.transform.position + new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2)));

        }
    }

    ////OnStateUpdate is handled in the AI_BaseClass////

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Doing this to make sure that the coroutine stops when it leaves the state
        //It may be redundant but it's a safety net because we aren't destroying the object when changing states
        Timing.KillCoroutines(moveTimer);

        //Stops all the movement to favor any other state
        agent.ResetPath();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //The moment the attacks are ready it will attack
        //animator.SetBool("GasAttackReady", mushroomData.canGasAttack);
        //animator.SetBool("OrbAttackReady", mushroomData.canOrbAttack);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        animator.GetComponentInParent<EnemyData>().RunCooldownTimer();
    }
    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
