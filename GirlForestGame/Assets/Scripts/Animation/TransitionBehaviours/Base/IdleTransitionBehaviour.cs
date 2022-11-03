using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleTransitionBehaviour : StateMachineBehaviour
{
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (InputManager.Instance.SwordAttack())
        {
            PlayerController.Instance.InitSwordAttack();
        }

        if (InputManager.Instance.ShootBow())
        {
            if (PlayerController.Instance.GetCanAttack())
            {
                PlayerController.Instance.BowAttack();
            }
        }
    }
}
