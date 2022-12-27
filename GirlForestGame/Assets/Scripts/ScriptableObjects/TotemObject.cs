using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TotemObject : ScriptableObject
{
    [SerializeReference] public Totem Totem;

    #region Menu Items
    [ContextMenu(nameof(HealthUp))] void HealthUp() { Totem = new HealthUpTotem(); }
    [ContextMenu(nameof(SwordDmgUp))] void SwordDmgUp() { Totem = new SwordDmgUpTotem(); }
    [ContextMenu(nameof(BowDmgUp))] void BowDmgUp() { Totem = new BowDmgUpTotem(); }
    [ContextMenu(nameof(CritUp))] void CritUp() { Totem = new CritUpTotem(); }
    [ContextMenu(nameof(SwordAtkSpdUp))] void SwordAtkSpdUp() { Totem = new SwordAtkSpdUpTotem(); }
    [ContextMenu(nameof(BowAtkSpdUp))] void BowAtkSpdUp() { Totem = new BowAtkSpdUpTotem(); }
    [ContextMenu(nameof(SpeedUp))] void SpeedUp() { Totem = new SpeedUpTotem(); }
    [ContextMenu(nameof(SwordRangeUp))] void SwordRangeUp() { Totem = new SwordRangeUpTotem(); }
    [ContextMenu(nameof(ProjSpdUp))] void ProjSpdUp() { Totem = new ProjSpdUpTotem(); }
    [ContextMenu(nameof(BowChargeSpdUp))] void BowChargeSpdUp() { Totem = new BowChargeSpdUpTotem(); }
    [ContextMenu(nameof(LuckUp))] void LuckUp() { Totem = new LuckUpTotem(); }
    //[ContextMenu(nameof(Berzerk))] void Berzerk() { Totem = new HealthUpTotem(); }
    //[ContextMenu(nameof(OneUp))] void OneUp() { Totem = new HealthUpTotem(); }
    //[ContextMenu(nameof(Executor))] void Executor() { Totem = new HealthUpTotem(); }
    //[ContextMenu(nameof(HealthyHitter))] void HealthyHitter() { Totem = new HealthUpTotem(); }
    //[ContextMenu(nameof(PlaneWalker))] void PlaneWalker() { Totem = new HealthUpTotem(); }
    //[ContextMenu(nameof(QuickDraw))] void QuickDraw() { Totem = new HealthUpTotem(); }
    //[ContextMenu(nameof(BladeMaster))] void BladeMaster() { Totem = new HealthUpTotem(); }
    //[ContextMenu(nameof(PlaneBuff))] void PlaneBuff() { Totem = new HealthUpTotem(); }
    //[ContextMenu(nameof(RabbitsFoot))] void RabbitsFoot() { Totem = new HealthUpTotem(); }
    #endregion
}

#region BaseTotems
[Serializable]
public class Totem
{
    public TotemTypes totemType;
    public string totemName;
    public float initialBuffAmount;
    public float stackDampenAmount;
    protected PlayerController player;
    protected int currentStackAmount;
    protected bool effectApplied;

    public virtual void Init()
    {
        player = PlayerController.Instance;
        effectApplied = false;
    }

    public virtual void ApplyEffect() { }

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

[Serializable]
public class OnTriggerTotem : Totem
{
    public override void Init()
    {
        base.Init();

        totemType = TotemTypes.OnTrigger;
    }
}

[Serializable]
public class ConstantTotem : Totem
{
    public bool conditionMet;

    public override void Init()
    {
        base.Init();

        totemType = TotemTypes.Constant;

        conditionMet = false;
    }

    public override void ApplyEffect()
    {
        CheckCondition();
    }

    public virtual void CheckCondition()
    {
        conditionMet = false;
    }
}

[Serializable]
public class OnPickupTotem : Totem
{
    public override void Init()
    {
        base.Init();

        totemType = TotemTypes.OnPickup;
    }
}
#endregion

#region Permanent Totems
[Serializable]
public class HealthUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        player.playerAttributes.MaxHealth += initialBuffAmount;
        player.playerAttributes.Health += initialBuffAmount;

        effectApplied = true;
    }
}

[Serializable]
public class SwordDmgUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.GetNumOfTotems(typeof(SwordDmgUpTotem));

        float damageToReduce = player.playerAttributes.PreviousSwordDamage * CalcBuffMultiplier(currentStackAmount - 1);

        player.playerAttributes.SwordDamage -= damageToReduce;
        player.playerAttributes.PreviousSwordDamage = player.playerAttributes.SwordDamage;
        player.playerAttributes.SwordDamage += player.playerAttributes.SwordDamage * CalcBuffMultiplier(currentStackAmount);

        effectApplied = true;
    }
}

[Serializable]
public class BowDmgUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.GetNumOfTotems(typeof(BowDmgUpTotem));
        Debug.Log($"CurrentStackAmount: {currentStackAmount}");

        float damageToReduce = player.playerAttributes.PreviousBowDamage * CalcBuffMultiplier(currentStackAmount - 1);

        Debug.Log($"Reduced Damage: {damageToReduce}");
        player.playerAttributes.BowDamage -= damageToReduce;
        player.playerAttributes.PreviousBowDamage = player.playerAttributes.BowDamage;

        Debug.Log($"Adding Damage: {player.playerAttributes.BowDamage * CalcBuffMultiplier(currentStackAmount)}");
        player.playerAttributes.BowDamage += player.playerAttributes.BowDamage * CalcBuffMultiplier(currentStackAmount);

        effectApplied = true;
    }
}

[Serializable]
public class CritUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        player.playerAttributes.CritChance += initialBuffAmount;

        effectApplied = true;
    }
}

[Serializable]
public class SwordAtkSpdUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.GetNumOfTotems(typeof(SwordAtkSpdUpTotem));

        float cooldownToAdd = player.playerAttributes.PreviousSwordCooldown * CalcBuffMultiplier(currentStackAmount - 1);

        player.playerAttributes.SwordCooldown += cooldownToAdd;
        player.playerAttributes.PreviousSwordCooldown = player.playerAttributes.SwordCooldown;
        player.playerAttributes.SwordCooldown -= player.playerAttributes.SwordCooldown * CalcBuffMultiplier(currentStackAmount);

        effectApplied = true;
    }
}

[Serializable]
public class BowAtkSpdUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.GetNumOfTotems(typeof(BowAtkSpdUpTotem));

        float cooldownToAdd = player.playerAttributes.PreviousBowCooldown * CalcBuffMultiplier(currentStackAmount - 1);

        player.playerAttributes.BowCooldown += cooldownToAdd;
        player.playerAttributes.PreviousBowCooldown = player.playerAttributes.BowCooldown;
        player.playerAttributes.BowCooldown -= player.playerAttributes.BowCooldown * CalcBuffMultiplier(currentStackAmount);

        effectApplied = true;
    }
}

[Serializable]
public class SpeedUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        player.playerAttributes.Speed += initialBuffAmount;

        effectApplied = true;
    }
}

[Serializable]
public class SwordRangeUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.GetNumOfTotems(typeof(SwordRangeUpTotem));

        float rangeToReduce = player.playerAttributes.PreviousRange * CalcBuffMultiplier(currentStackAmount - 1);

        player.playerAttributes.SwordRange -= rangeToReduce;
        player.playerAttributes.PreviousRange = player.playerAttributes.SwordRange;
        player.playerAttributes.SwordRange += player.playerAttributes.SwordRange * CalcBuffMultiplier(currentStackAmount);

        effectApplied = true;
    }
}

[Serializable]
public class ProjSpdUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        player.playerAttributes.ProjectileSpeed += initialBuffAmount;

        effectApplied = true;
    }
}

[Serializable]
public class BowChargeSpdUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        currentStackAmount = player.playerInventory.GetNumOfTotems(typeof(BowChargeSpdUpTotem));

        float timeToAdd = player.playerAttributes.PreviousChargeTime * CalcBuffMultiplier(currentStackAmount - 1);

        player.playerAttributes.BowChargeTime += timeToAdd;
        player.playerAttributes.PreviousChargeTime = player.playerAttributes.BowChargeTime;
        player.playerAttributes.BowChargeTime -= player.playerAttributes.BowChargeTime * CalcBuffMultiplier(currentStackAmount);

        effectApplied = true;
    }
}

[Serializable]
public class LuckUpTotem : OnPickupTotem
{
    public override void ApplyEffect()
    {
        player.playerAttributes.Luck += initialBuffAmount;

        effectApplied = true;
    }
}
#endregion

#region Constant Totems
public class BerzerkTotem : ConstantTotem
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();

        if (conditionMet && !effectApplied)
        {
            player.playerAttributes.SwordDamage *= 2;
            player.playerAttributes.BowDamage *= 2;
            player.playerAttributes.CritChance *= 2;

            effectApplied = true;
        }
        else if (!conditionMet && effectApplied)
        {
            player.playerAttributes.SwordDamage /= 2;
            player.playerAttributes.BowDamage /= 2;
            player.playerAttributes.CritChance /= 2;

            effectApplied = false;
        }
    }

    public override void CheckCondition()
    {
        if (player.playerAttributes.Health == 1)
        {
            conditionMet =  true;
        }

        conditionMet = false;
    }
}
#endregion

#region OnTrigger Totems
public class PlaneBuffTotem : OnTriggerTotem
{
    public override void Init()
    {
        base.Init();

        EventManager.OnPlaneSwitch += ApplyEffect;
    }

    public override void ApplyEffect()
    {
        if (player.Form == Forms.Living)
        {

        }
        else if(player.Form == Forms.Spirit)
        {

        }
    }
}

#endregion

public enum TotemTypes
{
    OnPickup,
    OnTrigger,
    Constant
}

public enum TotemList
{
    HealthUp,
    SwordDmgUp,
    BowDmgUp,
    CritUp,
    SwordAtkSpdUp,
    BowAtkSpdUp,
    SpeedUp,
    SwordRangeUp,
    ProjSpdUp,
    Berzerk,
    OneUp,
    Executor,
    HealthyHitter,
    PlaneWalker,
    QuickDraw,
    BladeMaster,
    PlaneBuff,
    RabbitsFoot
}
