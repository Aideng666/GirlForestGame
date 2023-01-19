using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "cdt", menuName = "Conditions/Base")]
abstract public class Cdt_BaseClass : ScriptableObject
{
    virtual public void CheckCondition(Animator animator, AI_BaseClass enemy = null)
    {
        //Checks and set Animator parameters here.
    }

    public virtual void ResetCondition(Animator animator)
    {

    }
}
