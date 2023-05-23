using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireBiteTotem : OnTriggerTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.totemDictionary[typeof(VampireBiteTotem)];

        float percentage = Random.Range(0f, 1f);

        if (percentage <= totemObject.initialBuffAmount * currentStackAmount)
        {
            player.playerAttributes.Health += 1;
        }
    }
}