using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstralBarrierTotem : ConstantTotem
{
    GameObject astralBarrier;
    float baseRadius;

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        currentStackAmount = player.playerInventory.totemDictionary[typeof(AstralBarrierTotem)];

        if (conditionMet && !effectApplied)
        {
            //Turn on Barrier
            if (astralBarrier == null)
            {
                astralBarrier = ParticleManager.Instance.SpawnParticle(ParticleTypes.AstralBarrier, player.transform.position);
                baseRadius = astralBarrier.GetComponent<SphereCollider>().radius;
            }
            else
            {
                astralBarrier.SetActive(true);
            }

            if (currentStackAmount != 1)
            {
                astralBarrier.GetComponent<SphereCollider>().radius = baseRadius;
                astralBarrier.GetComponent<SphereCollider>().radius += astralBarrier.GetComponent<SphereCollider>().radius * CalcBuffMultiplier(currentStackAmount);
            }

            effectApplied = true;
        }
        else if (!conditionMet && effectApplied)
        {
            //Turn off Barrier
            astralBarrier.SetActive(false);
            effectApplied = false;
        }
    }

    public override void CheckCondition()
    {
        if (player.playerCombat.Form == Planes.Astral)
        {
            conditionMet = true;

            return;
        }

        conditionMet = false;
    }
}
