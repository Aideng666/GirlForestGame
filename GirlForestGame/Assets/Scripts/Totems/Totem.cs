using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour
{
    [SerializeField] protected TotemTypes totemType;
    [SerializeField] protected Sprite totemSprite;
    [SerializeField] protected string totemName;
    [SerializeField] protected string totemDescription;
    [SerializeField] protected float initialBuffAmount;
    [SerializeField] protected float stackDampenAmount;
    protected PlayerController player;
    protected int currentStackAmount;
    protected float previousAmountAdded;
    public bool effectApplied { get; protected set; }

    public virtual void Init()
    {
        player = PlayerController.Instance;
        effectApplied = false;
    }

    public virtual void ApplyEffect() { }

    public virtual void RemoveEffect() { }

    public TotemTypes GetTotemType()
    {
        return totemType;
    }

    public float CalcBuffMultiplier(int stackAmount)
    {
        float multiplier = 0;

        if (stackDampenAmount > 0)
        {
            float amountToAdd = initialBuffAmount / stackDampenAmount;

            for (int i = 0; i < stackAmount; i++)
            {
                amountToAdd = amountToAdd * stackDampenAmount;

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

//public enum TotemTypes
//{
//    Permanent,
//    OnTrigger,
//    Constant
//}

