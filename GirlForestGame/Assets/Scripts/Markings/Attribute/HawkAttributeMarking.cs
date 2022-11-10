using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HawkAttributeMarking : AttributeMarking
{
    protected override void Start()
    {
        base.Start();

        buffedAttributes.Add(PlayerAttributes.AtkSpd);
        buffedAttributes.Add(PlayerAttributes.Speed);
    }
}
