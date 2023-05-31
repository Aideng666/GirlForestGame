using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjSpdUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        player.playerAttributes.ProjectileSpeed += totemObject.initialBuffAmount;

        effectApplied = true;
    }
}