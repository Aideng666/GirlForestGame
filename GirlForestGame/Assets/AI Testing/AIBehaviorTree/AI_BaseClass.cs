using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_BaseClass : StateMachineBehaviour
{
    [SerializeField]public List<Cdt_BaseClass> condition;
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Does the condition that is assigned as the condition variable (Can have multiple conditions)
        foreach (var cond in condition)
        {
            cond.CheckCondition(animator);
        }
    }
}
