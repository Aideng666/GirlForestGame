using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack2TransitionBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController.Instance.SetCanAttack(true, true, Weapons.Sword);

        PlayerController.Instance.SetCurrentAttackNum(3);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (InputManager.Instance.SwordAttack())
        {
            PlayerController.Instance.SwordAttack();
        }
    }
}
