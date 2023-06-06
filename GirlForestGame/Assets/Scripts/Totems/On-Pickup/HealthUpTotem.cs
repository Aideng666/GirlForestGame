using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        player.playerAttributes.MaxHealth += (int)totemObject.initialBuffAmount;
        player.playerAttributes.Health += (int)totemObject.initialBuffAmount;

        effectApplied = true;
    }
}
