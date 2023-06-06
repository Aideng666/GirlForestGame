using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeMasterTotem : OnTriggerTotem
{
    public override void ApplyEffect()
    {
        if (!effectApplied)
        {
            currentStackAmount = player.playerInventory.totemDictionary[typeof(BladeMasterTotem)];

            previousAmountAdded = player.playerAttributes.SwordDamage * CalcBuffMultiplier(currentStackAmount);
            player.playerAttributes.SwordDamage += player.playerAttributes.SwordDamage * CalcBuffMultiplier(currentStackAmount);

            effectApplied = true;
        }
    }

    public override void RemoveEffect()
    {
        if (effectApplied)
        {
            player.playerAttributes.SwordDamage -= previousAmountAdded;

            effectApplied = false;
        }
    }
}