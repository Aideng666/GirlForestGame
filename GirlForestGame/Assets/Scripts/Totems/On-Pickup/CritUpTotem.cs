using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        player.playerAttributes.CritChance += totemObject.initialBuffAmount;

        effectApplied = true;
    }
}
