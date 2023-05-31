using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordRangeUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.totemDictionary[typeof(SwordRangeUpTotem)];

        player.playerAttributes.SwordRange -= previousAmountAdded;

        previousAmountAdded = player.playerAttributes.SwordRange * CalcBuffMultiplier(currentStackAmount);
        player.playerAttributes.SwordRange += player.playerAttributes.SwordRange * CalcBuffMultiplier(currentStackAmount);

        effectApplied = true;
    }
}