using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    //For Player Attack Events
    public delegate void OnHitAttack(List<EnemyData> enemiesHit, Weapons weapon, bool guaranteeActivation = false);
    public static event OnHitAttack OnSwordHit;
    public static event OnHitAttack OnBowHit;

    //General Events
    public delegate void OnEvent();
    public static event OnEvent OnAttributeChange;
    public static event OnEvent OnHealthChange;

    public static EventManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

    public void InvokeOnSwordHit(List<EnemyData> enemiesHit, bool guaranteeActivation = false)
    {
        OnSwordHit?.Invoke(enemiesHit, Weapons.Sword, guaranteeActivation);
    }

    public void InvokeOnBowHit(List<EnemyData> enemiesHit, bool guaranteeActivation = false)
    {
        OnBowHit?.Invoke(enemiesHit, Weapons.Bow, guaranteeActivation);
    }

    public void InvokeOnAttributeChange(Attributes attribute)
    {
        OnAttributeChange?.Invoke();

        switch (attribute)
        {
            case Attributes.Health:

                OnHealthChange?.Invoke();

                break;
        }
    }
}

public enum TotemEvents
{
    OnEnemyKill,
    OnPlayerDeath,
    OnPlaneSwitch,
    OnSwordSwing
}
