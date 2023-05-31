using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAtkSpdUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.totemDictionary[typeof(BowAtkSpdUpTotem)];

        player.playerAttributes.BowCooldown += previousAmountAdded;

        previousAmountAdded = player.playerAttributes.BowCooldown * CalcBuffMultiplier(currentStackAmount);
        player.playerAttributes.BowCooldown -= player.playerAttributes.BowCooldown * CalcBuffMultiplier(currentStackAmount);

        effectApplied = true;
    }
}