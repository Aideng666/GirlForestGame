using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalAttackTransitionBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController player = PlayerController.Instance;

        //EventManager.Instance.InvokeTotemTrigger(TotemEvents.OnSwordSwing);
        if (player.playerInventory.totemDictionary[typeof(PlaneSwapEmpowermentTotem)] > 0)
        {
            player.playerInventory.GetTotemFromList(typeof(PlaneSwapEmpowermentTotem)).Totem.RemoveEffect();
        }

        if (player.playerInventory.totemDictionary[typeof(BladeMasterTotem)] > 0)
        {
            player.playerInventory.GetTotemFromList(typeof(BladeMasterTotem)).Totem.RemoveEffect();
        }

        //player.playerCombat.SetCanAttack(false, 0);

        //player.playerCombat.SetCanAttack(true, PlayerController.Instance.playerAttributes.SwordCooldown);

        player.playerCombat.SetCurrentAttackNum(1);

        animator.ResetTrigger("Attack3");

        Debug.Log("Reached Final Transition");
    }
}
