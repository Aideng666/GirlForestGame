using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAttackTransitionBehaviour : StateMachineBehaviour
{
    bool bowDrawn;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bowDrawn = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1 && !bowDrawn)
        {
            PlayerController.Instance.playerCombat.SetBowDrawn(true);

            bowDrawn = true;
        }
    }
}
