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
            EventManager.Instance.InvokeOnAttributeChange(Attributes.Health);} }
    public float Speed { get { return currentSpeed; } set { currentSpeed = value;
            EventManager.Instance.InvokeOnAttributeChange(Attributes.Speed);} }
    public float SwordDamage { get { return currentSwordDamage; } set { currentSwordDamage = value;
            EventManager.Instance.InvokeOnAttributeChange(Attributes.SwordDamage);} }
    public float BowDamage { get { return currentBowDamage; } set { currentBowDamage = value; 
            EventManager.Instance.InvokeOnAttributeChange(Attributes.BowDamage); } }
    public float SwordCooldown { get { return currentSwordCooldown; } set { currentSwordCooldown = value; 
            EventManager.Instance.InvokeOnAttributeChange(Attributes.SwordCooldown); } }
    public float BowCooldown { get { return currentBowCooldown; } set { currentBowCooldown = value; 
            EventManager.Instance.InvokeOnAttributeChange(Attributes.BowCooldown); } }
    public float SwordRange { get { return currentSwordRange; } set { currentSwordRange = value; 
            EventManager.Instance.InvokeOnAttributeChange(Attributes.SwordRange); } }
    public float ProjectileSpeed { get { return currentProjectileSpeed; } set { currentProjectileSpeed = value;
            if (currentProjectileSpeed > 99) currentProjectileSpeed = 99; 
            EventManager.Instance.InvokeOnAttributeChange(Attributes.ProjectileSpeed);} }
    public float CritChance { get { return currentCritChance; } set { currentCritChance = value; 
            EventManager.Instance.InvokeOnAttributeChange(Attributes.CritChance); } }
    public float BowChargeTime { get { return currentBowChargeTime; } set { currentBowChargeTime = value; 
            EventManager.Instance.InvokeOnAttributeChange(Attributes.BowChargeTime); } }
    public float Luck { get { return currentLuck; } set { currentLuck = value; 
            EventManager.Instance.InvokeOnAttributeChange(Attributes.Luck); } }

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

public enum Attributes
{
    Health,
    Speed,
    SwordDamage,
    BowDamage,
    SwordCooldown,
    BowCooldown,
    SwordRange,
    ProjectileSpeed,
    CritChance,
    BowChargeTime,
    Luck
}
