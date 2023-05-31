using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAtkSpdUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.totemDictionary[typeof(SwordAtkSpdUpTotem)];

        player.playerAttributes.SwordCooldown += previousAmountAdded;

        previousAmountAdded = player.playerAttributes.SwordCooldown * CalcBuffMultiplier(currentStackAmount);
        player.playerAttributes.SwordCooldown -= player.playerAttributes.SwordCooldown * CalcBuffMultiplier(currentStackAmount);

        effectApplied = true;
    }
}
