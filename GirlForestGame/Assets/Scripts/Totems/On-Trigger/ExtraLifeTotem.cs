using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLifeTotem : OnTriggerTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.totemDictionary[typeof(ExtraLifeTotem)];

        player.playerAttributes.Health += (int)totemObject.initialBuffAmount * currentStackAmount;
    }
}
