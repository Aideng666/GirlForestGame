using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttributes : MonoBehaviour
{
    [Header("Default Player Attributes")]
    [SerializeField] int defaultHealth = 6;
    [SerializeField] int maximumHealth = 12;
    [SerializeField] float defaultSpeed;
    [SerializeField] float defaultSwordDamage;
    [SerializeField] float defaultBowDamage;
    [SerializeField] float defaultSwordCooldown;
    [SerializeField] float defaultBowCooldown;
    [SerializeField] float defaultSwordRange = 5;
    [SerializeField] float defaultProjectileSpeed;
    [SerializeField] float defaultCritChance = 0.05f; // between 0 and 1 for 0%-100%
    [SerializeField] float defaultBowChargeTime = 3;
    [SerializeField] float defaultLuck = 0;
    int currentMaxHealth;
    int currentHealth;
    float currentSpeed;
    float currentSwordDamage;
    float currentBowDamage;
    float currentSwordCooldown;
    float currentBowCooldown;
    float currentSwordRange;
    float currentProjectileSpeed;
    float currentCritChance;
    float currentBowChargeTime;
    float currentLuck;

    public int MaxHealth { get { return currentMaxHealth; } set { currentMaxHealth = value;
            if (currentMaxHealth > maximumHealth) currentMaxHealth = maximumHealth; } }
    public int Health { get { return currentHealth; } set { currentHealth = value;
            if (currentHealth > currentMaxHealth) currentHealth = currentMaxHealth;
            if (currentHealth < 0) currentHealth = 0;
            EventManager.Instance.InvokeOnHealthChange();
        } }
    public float Speed { get { return currentSpeed; } set { currentSpeed = value; } }
    public float SwordDamage { get { return currentSwordDamage; } set { currentSwordDamage = value; } }
    public float BowDamage { get { return currentBowDamage; } set { currentBowDamage = value; } }
    public float SwordCooldown { get { return currentSwordCooldown; } set { currentSwordCooldown = value; } }
    public float BowCooldown { get { return currentBowCooldown; } set { currentBowCooldown = value; } }
    public float SwordRange { get { return currentSwordRange; } set { currentSwordRange = value; } }
    public float ProjectileSpeed { get { return currentProjectileSpeed; } set { currentProjectileSpeed = value;
            if (currentProjectileSpeed > 99) currentProjectileSpeed = 99;} }
    public float CritChance { get { return currentCritChance; } set { currentCritChance = value; } }
    public float BowChargeTime { get { return currentBowChargeTime; } set { currentBowChargeTime = value; } }
    public float Luck { get { return currentLuck; } set { currentLuck = value; } }

    private void Start()
    {
        currentMaxHealth = defaultHealth;
        currentHealth = defaultHealth;
        currentSpeed = defaultSpeed;
        currentSwordDamage = defaultSwordDamage;
        currentBowDamage = defaultBowDamage;
        currentSwordCooldown = defaultSwordCooldown;
        currentBowCooldown = defaultBowCooldown;
        currentSwordRange = defaultSwordRange;
        currentProjectileSpeed = defaultProjectileSpeed;
        currentCritChance = defaultCritChance;
        currentBowChargeTime = defaultBowChargeTime;
        currentLuck = defaultLuck;
    }
}
