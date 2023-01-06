using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDelay2TransitionBehaviour : StateMachineBehaviour
{
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PlayerController.Instance.playerCombat.GetCanAttack())
        {
            PlayerController.Instance.playerCombat.InitSwordAttack();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (PlayerController.Instance.playerInventory.totemDictionary[typeof(BladeMasterTotem)] > 0)
        {
            PlayerController.Instance.playerInventory.GetTotemFromList(typeof(BladeMasterTotem)).Totem.ApplyEffect();
        }
    }
}
