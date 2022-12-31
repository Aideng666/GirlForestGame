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
    public delegate void OnTotemTrigger();
    public static event OnTotemTrigger OnExecute;
    public static event OnTotemTrigger OnPlayerDeath;

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

    public void InvokeTotemTrigger(TotemList totem)
    {
        switch (totem)
        {
            case TotemList.Executor:

                OnExecute?.Invoke();

                break;

            case TotemList.ExtraLife:

                OnPlayerDeath?.Invoke();

                break;
        }
    }
}
