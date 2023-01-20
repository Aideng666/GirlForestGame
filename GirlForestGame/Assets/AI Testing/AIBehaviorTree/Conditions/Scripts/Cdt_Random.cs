using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Random", menuName = "Conditions/Random")]
public class Cdt_Random : Cdt_BaseClass
{

    [SerializeField] List<string> Condition_Parameters;
    public override void CheckCondition(Animator animator)
    {
        //Picks a random state from the list
        animator.SetTrigger(Condition_Parameters[Random.Range(0, Condition_Parameters.Count)]);
    }
}
