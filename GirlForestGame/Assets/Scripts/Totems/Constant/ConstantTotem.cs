using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantTotem : Totem
{
    [HideInInspector]
    public bool conditionMet;

    public override void Init()
    {
        base.Init();

        totemType = TotemTypes.Constant;

        conditionMet = false;
    }

    public override void ApplyEffect()
    {
        CheckCondition();
    }

    public virtual void CheckCondition()
    {
        conditionMet = false;
    }
}
