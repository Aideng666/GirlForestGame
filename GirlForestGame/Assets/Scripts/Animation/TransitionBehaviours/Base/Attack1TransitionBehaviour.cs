using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack1TransitionBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController player = PlayerController.Instance;

        if (player.playerInventory.totemDictionary[typeof(PlaneSwapEmpowermentTotem)] > 0)
        {
            player.playerInventory.GetTotemFromList(typeof(PlaneSwapEmpowermentTotem)).Totem.RemoveEffect();
        }

        player.playerCombat.SetCanAttack(true, true, Weapons.Sword);

        player.playerCombat.SetCurrentAttackNum(2);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (InputManager.Instance.SwordAttack())
        {
            animator.SetTrigger("Attack2");
        }
    }
}
