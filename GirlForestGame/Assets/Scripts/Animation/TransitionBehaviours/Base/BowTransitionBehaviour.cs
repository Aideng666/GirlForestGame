using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowTransitionBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerController player = PlayerController.Instance;

        player.playerCombat.SetCanAttack(false, 0);

        player.playerCombat.SetCanAttack(true, player.playerAttributes.BowCooldown);

        if (player.playerInventory.totemDictionary[typeof(PlaneSwapEmpowermentTotem)] > 0)
        {
            player.playerInventory.GetTotemFromList(typeof(PlaneSwapEmpowermentTotem)).Totem.RemoveEffect();
        }
    }
}
