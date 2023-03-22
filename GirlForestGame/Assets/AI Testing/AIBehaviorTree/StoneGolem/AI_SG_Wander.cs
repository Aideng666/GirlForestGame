using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_SG_Wander : AI_BaseClass
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        agent.speed = 2;
        agent.isStopped = false;

    }
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        //Targets the player
        if (agent.enabled)
        {
            agent.SetDestination(player.transform.position);
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // agent.SetDestination(agent.transform.position); //If it leaves for any reason it's current location is the thing
        agent.isStopped = true;
    }
}
