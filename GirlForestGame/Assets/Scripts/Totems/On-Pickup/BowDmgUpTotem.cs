using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowDmgUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.totemDictionary[typeof(BowDmgUpTotem)];

        player.playerAttributes.BowDamage -= previousAmountAdded;

        previousAmountAdded = player.playerAttributes.BowDamage * CalcBuffMultiplier(currentStackAmount);
        player.playerAttributes.BowDamage += player.playerAttributes.BowDamage * CalcBuffMultiplier(currentStackAmount);

        effectApplied = true;
    }
}
