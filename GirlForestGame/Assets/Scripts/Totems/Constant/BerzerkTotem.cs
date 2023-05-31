using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerzerkTotem : ConstantTotem
{
    private float previousSwdDmgAdded;
    private float previousBowDmgAdded;
    private float previousCritChanceAdded;

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        currentStackAmount = player.playerInventory.totemDictionary[typeof(BerzerkTotem)];

        if (conditionMet && !effectApplied)
        {
            previousSwdDmgAdded = player.playerAttributes.SwordDamage * CalcBuffMultiplier(currentStackAmount);
            previousBowDmgAdded = player.playerAttributes.BowDamage * CalcBuffMultiplier(currentStackAmount);
            previousCritChanceAdded = player.playerAttributes.CritChance * CalcBuffMultiplier(currentStackAmount);

            player.playerAttributes.SwordDamage += player.playerAttributes.SwordDamage * CalcBuffMultiplier(currentStackAmount);
            player.playerAttributes.BowDamage += player.playerAttributes.BowDamage * CalcBuffMultiplier(currentStackAmount);
            player.playerAttributes.CritChance += player.playerAttributes.CritChance * CalcBuffMultiplier(currentStackAmount);

            effectApplied = true;
        }
        else if (!conditionMet && effectApplied)
        {
            player.playerAttributes.SwordDamage -= previousSwdDmgAdded;
            player.playerAttributes.BowDamage -= previousBowDmgAdded;
            player.playerAttributes.CritChance -= previousCritChanceAdded;

            effectApplied = false;
        }
    }

    public override void CheckCondition()
    {
        if (player.playerAttributes.Health == 1)
        {
            conditionMet = true;
        }

        conditionMet = false;
    }
}
