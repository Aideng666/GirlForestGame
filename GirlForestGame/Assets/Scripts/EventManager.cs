using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //For Player Attack Events
    public delegate void OnHitAttack(List<Enemy> enemiesHit, Weapons weapon);
    public static event OnHitAttack OnSwordHit;
    public static event OnHitAttack OnBowHit;

    //For OnTrigger Totem events
    //public delegate void OnTotemTrigger();
    //public static event OnTotemTrigger OnEnemyKill;
    //public static event OnTotemTrigger OnPlayerDeath;
    //public static event OnTotemTrigger OnPlaneSwitch;
    //public static event OnTotemTrigger OnSwordSwing;

    public static EventManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

    public void InvokeOnSwordHit(List<Enemy> enemiesHit)
    {
        OnSwordHit?.Invoke(enemiesHit, Weapons.Sword);
    }

    public void InvokeOnBowHit(List<Enemy> enemiesHit)
    {
        OnBowHit?.Invoke(enemiesHit, Weapons.Bow);
    }

    //public void InvokeTotemTrigger(TotemEvents totemEvent)
    //{
    //    switch (totemEvent)
    //    {
    //        case TotemEvents.OnEnemyKill:

    //            OnEnemyKill?.Invoke();

    //            break;

    //        case TotemEvents.OnPlayerDeath:

    //            OnPlayerDeath?.Invoke();

    //            break;

    //        case TotemEvents.OnPlaneSwitch:

    //            OnPlaneSwitch?.Invoke();

    //            break;

    //        case TotemEvents.OnSwordSwing:

    //            OnSwordSwing?.Invoke();

    //            break;
    //    }
    //}
}

public enum TotemEvents
{
    OnEnemyKill,
    OnPlayerDeath,
    OnPlaneSwitch,
    OnSwordSwing
}
