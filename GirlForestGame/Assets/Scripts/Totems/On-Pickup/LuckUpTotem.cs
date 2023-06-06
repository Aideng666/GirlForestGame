using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        player.playerAttributes.Luck += totemObject.initialBuffAmount;

        effectApplied = true;
    }
}
