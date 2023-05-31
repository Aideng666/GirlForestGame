using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowChargeSpdUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.totemDictionary[typeof(BowChargeSpdUpTotem)];

        player.playerAttributes.BowChargeTime += previousAmountAdded;

        previousAmountAdded = player.playerAttributes.BowChargeTime * CalcBuffMultiplier(currentStackAmount);
        player.playerAttributes.BowChargeTime -= player.playerAttributes.BowChargeTime * CalcBuffMultiplier(currentStackAmount);

        effectApplied = true;
    }
}