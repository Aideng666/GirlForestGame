using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        player.playerAttributes.Speed += totemObject.initialBuffAmount;

        effectApplied = true;
    }
}