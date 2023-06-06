using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSwapEmpowermentTotem : OnTriggerTotem
{
    float previousSwordAmountAdded;
    float previousBowAmountAdded;

    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.totemDictionary[typeof(PlaneSwapEmpowermentTotem)];

        if (player.playerCombat.Form == Planes.Terrestrial)
        {
            previousSwordAmountAdded = player.playerAttributes.SwordDamage * CalcBuffMultiplier(currentStackAmount);
            player.playerAttributes.SwordDamage += player.playerAttributes.SwordDamage * CalcBuffMultiplier(currentStackAmount);
        }
        else if (player.playerCombat.Form == Planes.Astral)
        {
            previousBowAmountAdded = player.playerAttributes.BowDamage * CalcBuffMultiplier(currentStackAmount);
            player.playerAttributes.BowDamage += player.playerAttributes.BowDamage * CalcBuffMultiplier(currentStackAmount);
        }

        effectApplied = true;
    }

    public override void RemoveEffect()
    {
        if (effectApplied)
        {
            if (player.playerCombat.Form == Planes.Terrestrial)
            {
                player.playerAttributes.SwordDamage -= previousSwordAmountAdded;
            }
            else if (player.playerCombat.Form == Planes.Astral)
            {
                player.playerAttributes.BowDamage -= previousBowAmountAdded;
            }

            effectApplied = false;
        }
    }
}
