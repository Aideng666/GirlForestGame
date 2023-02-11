using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The base class for any AI action. It loops through all the conditions and will check if it is being met
/// </summary>
public class AI_BaseClass : StateMachineBehaviour
{
    protected NavMeshAgent agent;
    protected PlayerController player;
    protected EnemyData enemyData;
    protected EnemyUI enemyUI;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponentInParent<NavMeshAgent>();
        agent.enabled = true;

        player = PlayerController.Instance;
        enemyData = animator.GetComponentInParent<EnemyData>();
        enemyUI = animator.transform.parent.GetComponentInChildren<EnemyUI>();
    }

    [SerializeField]public List<Cdt_BaseClass> conditions;
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Does the condition that is assigned as the condition variable (Can have multiple conditions)
        foreach (var cond in conditions)
        {
            cond.CheckCondition(animator);
        }
    }
}
