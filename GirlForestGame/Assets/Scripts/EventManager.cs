using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void OnHitAttack(List<Enemy> enemiesHit);
    public static event OnHitAttack OnSwordHit;
    public static event OnHitAttack OnBowHit;

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
        OnSwordHit?.Invoke(enemiesHit);
    }

    public void InvokeOnBowHit(List<Enemy> enemiesHit)
    {
        OnBowHit?.Invoke(enemiesHit);
    }
}
