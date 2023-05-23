using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour
{
    //public TotemTypes totemType;
    //public Sprite totemSprite;
    //public string totemName;
    //public string totemDescription;
    //public float initialBuffAmount;
    //public float stackDampenAmount;
    [HideInInspector]
    public TotemObject totemObject;
    protected PlayerController player;
    protected int currentStackAmount;
    protected float previousAmountAdded;
    public bool effectApplied { get; protected set; }

    public virtual void Init()
    {
        player = PlayerController.Instance;
        effectApplied = false;

        foreach (TotemObject totem in TypeHandler.GetAllInstances<TotemObject>("Totems"))
        {
            if (totem.totemName == GetType().ToString())
            {
                totemObject = totem;

                print($"Totem Created: {totemObject.totemName}");

                break;
            }
        }
    }

    public virtual void ApplyEffect() { }

    public virtual void RemoveEffect() { }

    public TotemTypes GetTotemType()
    {
        return totemObject.totemType;
    }

    public float CalcBuffMultiplier(int stackAmount)
    {
        float multiplier = 0;

        if (totemObject.stackDampenAmount > 0)
        {
            float amountToAdd = totemObject.initialBuffAmount / totemObject.stackDampenAmount;

            for (int i = 0; i < stackAmount; i++)
            {
                amountToAdd = amountToAdd * totemObject.stackDampenAmount;

                multiplier += amountToAdd;
            }
        }
        else
        {
            return 1;
        }

        return multiplier;
    }
}

public enum TotemTypes
{
    OnPickup,
    OnTrigger,
    Constant
}

