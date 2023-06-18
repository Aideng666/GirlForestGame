using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearfulAuraTotem : ConstantTotem
{
    [SerializeField] float basePulseTimer = 5;
    float elaspedTime = 0;
    GameObject fearParticle = null;

    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.totemDictionary[typeof(AstralBarrierTotem)];

        if (elaspedTime >= basePulseTimer - ((currentStackAmount - 1) * totemObject.initialBuffAmount))
        {
            if (fearParticle == null)
            {
                fearParticle = ParticleManager.Instance.SpawnParticle(ParticleTypes.FearfulAura, player.transform.position);
            }
            else
            {
                fearParticle.SetActive(true);
            }
            FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Totem/Fear");

            elaspedTime = 0;
        }

        elaspedTime += Time.deltaTime;
    }

   
}
