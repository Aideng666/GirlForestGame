using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalAttackTransitionBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController.Instance.playerCombat.SetCanAttack(false, false);

        PlayerController.Instance.playerCombat.SetCanAttack(true, true, Weapons.Sword);

        PlayerController.Instance.playerCombat.SetCurrentAttackNum(1);

        animator.ResetTrigger("Attack3");
    }
}
