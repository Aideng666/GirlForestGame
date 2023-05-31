using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrestrialShieldTotem : ConstantTotem
{
    [SerializeField] GameObject shieldObject;
    float baseCooldownTimer = 3;
    bool shieldCreated = false;
    TerrestrialShieldObject shield;
    float elaspedCooldownTime = 0;

    public override void ApplyEffect()
    {
        base.ApplyEffect();

        currentStackAmount = player.playerInventory.totemDictionary[typeof(TerrestrialShieldTotem)];

        if (conditionMet)
        {
            if (!shieldCreated)
            {
                shieldObject = Instantiate(shieldObject, player.transform.position + (Vector3.forward * 2) + Vector3.up, Quaternion.identity);
                shield = shieldObject.GetComponent<TerrestrialShieldObject>();

                shieldCreated = true;
            }

            if (!shield.GetCooldownApplied())
            {
                shieldObject.SetActive(true);
            }
            else if (elaspedCooldownTime >= baseCooldownTimer - ((currentStackAmount - 1) * totemObject.initialBuffAmount)/* || !shield.GetCooldownApplied()*/)
            {
                shieldObject.SetActive(true);

                elaspedCooldownTime = 0;
                shield.SetCooldownApplied(false);
            }
            else
            {
                elaspedCooldownTime += Time.deltaTime;
            }
        }
        else if (!conditionMet)
        {
            if (shieldObject.activeInHierarchy)
            {
                shield.SetCooldownApplied(false);
                shieldObject.SetActive(false);
            }
        }
    }

    public override void CheckCondition()
    {
        if (player.playerCombat.Form == Planes.Terrestrial)
        {
            conditionMet = true;

            return;
        }

        conditionMet = false;
    }

}