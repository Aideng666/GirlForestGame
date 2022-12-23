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
    protected PlayerController player;

    protected virtual void Init()
    {
        player = PlayerController.Instance;
    }

    public virtual void ApplyEffect() { }

    public TotemTypes GetTotemType()
    {
        return totemType;
    }
}

[Serializable]
public class OnTriggerTotem : Totem
{
    protected override void Init()
    {
        base.Init();

        totemType = TotemTypes.OnTrigger;
    }
}

[Serializable]
public class ConstantTotem : Totem
{
    protected override void Init()
    {
        base.Init();

        totemType = TotemTypes.Constant;
    }

    //// Update is called once per frame
    //protected void Update()
    //{
    //    //if (totemInInventory)
    //    //{
    //    //    ApplyEffect();
    //    //}
    //}
}

[Serializable]
public class PermanentTotem : Totem
{
    public float buffAmount;

    protected override void Init()
    {
        base.Init();

        totemType = TotemTypes.Permanent;
    }
}
#endregion

#region Permanent Totems
[Serializable]
public class HealthUpTotem : PermanentTotem
{
    public HealthUpTotem()
    {
        Init();
    }

    public override void ApplyEffect()
    {
        player.playerAttributes.MaxHealth += buffAmount;
        player.playerAttributes.Health += buffAmount;
    }
}

[Serializable]
public class SwordDmgUpTotem : PermanentTotem
{
    public SwordDmgUpTotem()
    {
        Init();
    }

    public override void ApplyEffect()
    {
        player.playerAttributes.SwordDamage += buffAmount;
    }
}

[Serializable]
public class BowDmgUpTotem : PermanentTotem
{
    public BowDmgUpTotem()
    {
        Init();
    }

    public override void ApplyEffect()
    {
        player.playerAttributes.BowDamage += buffAmount;
    }
}

[Serializable]
public class CritUpTotem : PermanentTotem
{
    public CritUpTotem()
    {
        Init();
    }

    public override void ApplyEffect()
    {
        player.playerAttributes.CritChance += buffAmount;
    }
}

[Serializable]
public class SwordAtkSpdUpTotem : PermanentTotem
{
    public SwordAtkSpdUpTotem()
    {
        Init();
    }

    public override void ApplyEffect()
    {
        player.playerAttributes.SwordCooldown -= buffAmount;
    }
}

[Serializable]
public class BowAtkSpdUpTotem : PermanentTotem
{
    public BowAtkSpdUpTotem()
    {
        Init();
    }

    public override void ApplyEffect()
    {
        player.playerAttributes.BowCooldown -= buffAmount;
    }
}

[Serializable]
public class SpeedUpTotem : PermanentTotem
{
    public SpeedUpTotem()
    {
        Init();
    }

    public override void ApplyEffect()
    {
        player.playerAttributes.Speed += buffAmount;
    }
}

[Serializable]
public class SwordRangeUpTotem : PermanentTotem
{
    public SwordRangeUpTotem()
    {
        Init();
    }

    public override void ApplyEffect()
    {
        player.playerAttributes.SwordRange += buffAmount;
    }
}

[Serializable]
public class ProjSpdUpTotem : PermanentTotem
{
    public ProjSpdUpTotem()
    {
        Init();
    }

    public override void ApplyEffect()
    {
        player.playerAttributes.ProjectileSpeed += buffAmount;
    }
}
#endregion

public enum TotemTypes
{
    Permanent,
    OnTrigger,
    Constant
}

//public enum TotemList
//{
//    HealthUp,
//    SwordDmgUp,
//    BowDmgUp,
//    CritUp,
//    SwordAtkSpdUp,
//    BowAtkSpdUp,
//    SpeedUp,
//    SwordRangeUp,
//    ProjSpdUp,
//    Berzerk,
//    OneUp,
//    Executor,
//    HealthyHitter,
//    PlaneWalker,
//    QuickDraw,
//    BladeMaster,
//    PlaneBuff,
//    RabbitsFoot
//}
