using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Distance_Check", menuName = "Conditions/Hit Tracking")]
public class Cdt_HasBeenHit : Cdt_BaseClass
{
    //public bool Has_Been_Hit = false;

    //DESCRIPTION: Condition for when the AI is hit so that it transitions to the hit animation

    public string triggerParameter = "Is_Hit";

    public override void CheckCondition(Animator animator, AI_BaseClass enemy = null)
    {

        //
        //if () 
        //{
        //    animator.SetTrigger(triggerParameter);
        //}
    }
}