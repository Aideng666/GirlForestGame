using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerTotem : Totem
{
    public override void Init()
    {
        base.Init();

        totemType = TotemTypes.OnTrigger;
    }
}
