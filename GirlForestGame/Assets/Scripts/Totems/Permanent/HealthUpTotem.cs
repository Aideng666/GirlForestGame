using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpTotem : PermanentTotem
{

    protected override void Start()
    {
        base.Start();

        totemName = "Health Up";
    }

    public override void ApplyEffect()
    {
        player.playerAttributes.MaxHealth += 2;
        player.playerAttributes.Health += 2;

        print("Effect Applied");
    }
}
