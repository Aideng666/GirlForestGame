using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PermanentTotem : Totem
{
    protected bool effectApplied;

    protected override void Start()
    {
        base.Start();

        totemType = TotemTypes.Permanent;
    }

    public virtual void ApplyEffect()
    {
     
    }
}
