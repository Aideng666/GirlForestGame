using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerTotem : Totem
{
    protected override void Start()
    {
        base.Start();

        totemType = TotemTypes.OnTrigger;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void ApplyEffect()
    {
        base.ApplyEffect();
    }
}
