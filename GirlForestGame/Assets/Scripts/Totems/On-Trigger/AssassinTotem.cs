using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinTotem : OnTriggerTotem
{
    Weapons weaponUsed;
    float previousSwordAmountAdded;
    float previousBowAmountAdded;

    public void SetWeaponUsed(Weapons weapon)
    {
        weaponUsed = weapon;

        ApplyEffect();
    }

    public override void ApplyEffect()
    {
        if (!effectApplied)
        {
            currentStackAmount = player.playerInventory.totemDictionary[typeof(AssassinTotem)];

            if (weaponUsed == Weapons.Sword)
            {
                previousSwordAmountAdded = player.playerAttributes.SwordDamage * CalcBuffMultiplier(currentStackAmount);
                player.playerAttributes.SwordDamage += player.playerAttributes.SwordDamage * CalcBuffMultiplier(currentStackAmount);
            }
            else if (weaponUsed == Weapons.Bow)
            {
                previousBowAmountAdded = player.playerAttributes.BowDamage * CalcBuffMultiplier(currentStackAmount);
                player.playerAttributes.BowDamage += player.playerAttributes.BowDamage * CalcBuffMultiplier(currentStackAmount);
            }

            effectApplied = true;
        }
    }

    public override void RemoveEffect()
    {
        if (effectApplied)
        {
            if (weaponUsed == Weapons.Sword)
            {
                player.playerAttributes.SwordDamage -= previousSwordAmountAdded;
            }
            else if (weaponUsed == Weapons.Bow)
            {
                player.playerAttributes.BowDamage -= previousBowAmountAdded;
            }

            effectApplied = false;
        }
    }

}
