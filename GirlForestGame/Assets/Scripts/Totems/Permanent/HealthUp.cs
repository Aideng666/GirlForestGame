using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUp : PermanentTotem
{

    protected override void Start()
    {
        base.Start();

        totemName = "Health Up";
    }

    public override void ApplyEffect()
    {
        if (!effectApplied)
        {
            player.playerAttributes.MaxHealth += 2;

            effectApplied = true;

            player.AddTotemToList(this);
        }
    }
}
