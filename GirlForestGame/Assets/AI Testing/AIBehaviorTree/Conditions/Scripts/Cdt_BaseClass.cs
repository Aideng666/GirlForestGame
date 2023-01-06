using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "cdt", menuName = "Conditions/Base")]
abstract public class Cdt_BaseClass : ScriptableObject
{
    virtual public void CheckCondition(Animator animator)
    {
        //Checks and set Animator parameters here.
    }
    virtual public void CheckCondition(Animator animator, AI_BaseClass enemy)
    {
        //Checks and set Animator parameters here. and has the enemy as a potential override
    }
}
