using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDmgUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.totemDictionary[typeof(SwordDmgUpTotem)];

        //Subtracting the previous buff because the new one is stacked
        player.playerAttributes.SwordDamage -= previousAmountAdded;

        //Adding the new Buff
        previousAmountAdded = player.playerAttributes.SwordDamage * CalcBuffMultiplier(currentStackAmount);
        player.playerAttributes.SwordDamage += player.playerAttributes.SwordDamage * CalcBuffMultiplier(currentStackAmount);

        effectApplied = true;
    }
}
