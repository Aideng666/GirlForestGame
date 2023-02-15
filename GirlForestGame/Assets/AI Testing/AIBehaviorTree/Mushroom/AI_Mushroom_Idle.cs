using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Mushroom_Idle : AI_BaseClass
{
    [SerializeField] float activationDistance = 4;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(animator.transform.position, player.transform.position) < activationDistance)
        {
            animator.SetTrigger("Awaken_From_Idle");
        }
    }
}
