using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMarkings : MonoBehaviour
{
    [Header("Fox Element Variables")]
    [SerializeField] int numberOfBurnTicks = 5;
    [SerializeField] float baseBurnTickDamage = 1;
    [SerializeField] float burnTickDelay = 1.1f;

    [Space(1)]

    [Header("Hawk Element Variables")]
    [SerializeField] float baseWindedDuration = 2;
    float windedDuration;

    public float BaseWindedDuration { get { return baseWindedDuration; } private set { } }

    [Space(1)]

    [Header("Level Multipliers")]
    [SerializeField] float[] attributeMultipliers = new float[3];
    [SerializeField] float[] elementMultipliers = new float[3];
    [SerializeField] float[] elementActivationChances = new float[3];

    public float[] ElementMultipliers { get { return elementMultipliers; } private set { } }

    //to access the player's scripts
    //The player controller contains access to the other player scripts
    PlayerController player;

    //0 = Sword Attribute
    //1 = Sword Element
    //2 = Bow Attribute
    //3 = Bow Element
    public Spirit[] markings { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();

        markings = new Spirit[] { null, null, null, null };
    }

    private void OnDisable()
    {
        EventManager.OnSwordHit -= ApplyWindElement;
        EventManager.OnSwordHit -= ApplyFireElement;
        EventManager.OnBowHit -= ApplyFireElement;
        EventManager.OnBowHit -= ApplyWindElement;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Called every time the player picks up or swaps a marking
    //Adds the correct stat bonuses to the player if it is an attribute marking
    //Applies the correct element onto the chosen weapon if it is an element marking
    public void UpdateMarking(Spirit spirit, MarkingTypes type, Weapons weapon)
    {
        //Updates the Attributes / Elements
        if (type == MarkingTypes.Attribute)
        {
            for (int i = 0; i < spirit.buffedAttributes.Count; i++)
            {
                switch (spirit.buffedAttributes[i])
                {
                    case Attributes.Health:

                        player.playerAttributes.MaxHealth += spirit.markingLevel + 1;

                        break;

                    case Attributes.SwordDamage:

                        player.playerAttributes.SwordDamage *= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.BowDamage:

                        player.playerAttributes.BowDamage *= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.SwordCooldown:

                        player.playerAttributes.SwordCooldown /= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.BowCooldown:

                        player.playerAttributes.BowCooldown /= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.Speed:

                        player.playerAttributes.Speed *= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.ProjectileSpeed:

                        player.playerAttributes.ProjectileSpeed *= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.SwordRange:

                        player.playerAttributes.SwordRange *= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.CritChance:

                        player.playerAttributes.CritChance *= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.BowChargeTime:

                        player.playerAttributes.BowChargeTime /= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.Luck:

                        player.playerAttributes.Luck *= attributeMultipliers[spirit.markingLevel - 1];

                        break;
                }
            }

            //Updates the local array for the player's equipped markings
            if (weapon == Weapons.Sword)
            {
                markings[0] = spirit;
            }
            else if (weapon == Weapons.Bow)
            {
                markings[2] = spirit;
            }
        }
        if (type == MarkingTypes.Element)
        {
            switch (spirit.usedElement)
            {
                case Elements.Wind:

                    if (weapon == Weapons.Sword)
                    {
                        EventManager.OnSwordHit += ApplyWindElement;
                    }
                    else if (weapon == Weapons.Bow)
                    {
                        EventManager.OnBowHit += ApplyWindElement;
                    }

                    break;

                case Elements.Fire:

                    if (weapon == Weapons.Sword)
                    {
                        EventManager.OnSwordHit += ApplyFireElement;
                    }
                    else if (weapon == Weapons.Bow)
                    {
                        EventManager.OnBowHit += ApplyFireElement;
                    }

                    break;
            }

            if (weapon == Weapons.Sword)
            {
                markings[1] = spirit;
            }
            else if (weapon == Weapons.Bow)
            {
                markings[3] = spirit;
            }
        }
    }

    public void RemoveMarking(Spirit spirit, MarkingTypes type, Weapons weapon)
    {
        if (type == MarkingTypes.Attribute)
        {
            for (int i = 0; i < spirit.buffedAttributes.Count; i++)
            {
                switch (spirit.buffedAttributes[i])
                {
                    case Attributes.Health:

                        player.playerAttributes.MaxHealth -= spirit.markingLevel + 1;

                        break;

                    case Attributes.SwordDamage:

                        player.playerAttributes.SwordDamage /= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.BowDamage:

                        player.playerAttributes.BowDamage /= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.SwordCooldown:

                        player.playerAttributes.SwordCooldown *= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.BowCooldown:

                        player.playerAttributes.BowCooldown *= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.Speed:

                        player.playerAttributes.Speed /= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.ProjectileSpeed:

                        player.playerAttributes.ProjectileSpeed /= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.SwordRange:

                        player.playerAttributes.SwordRange /= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.CritChance:

                        player.playerAttributes.CritChance /= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.BowChargeTime:

                        player.playerAttributes.BowChargeTime *= attributeMultipliers[spirit.markingLevel - 1];

                        break;

                    case Attributes.Luck:

                        player.playerAttributes.Luck /= attributeMultipliers[spirit.markingLevel - 1];

                        break;
                }
            }
        }
        if (type == MarkingTypes.Element)
        {
            switch (spirit.usedElement)
            {
                case Elements.Wind:

                    if (weapon == Weapons.Sword)
                    {
                        EventManager.OnSwordHit -= ApplyWindElement;
                    }
                    else if (weapon == Weapons.Bow)
                    {
                        EventManager.OnBowHit -= ApplyWindElement;
                    }

                    break;

                case Elements.Fire:

                    if (weapon == Weapons.Sword)
                    {
                        EventManager.OnSwordHit -= ApplyFireElement;
                    }
                    else if (weapon == Weapons.Bow)
                    {
                        EventManager.OnBowHit -= ApplyFireElement;
                    }

                    break;
            }
        }
    }

    void ApplyFireElement(List<EnemyData> enemiesHit, Weapons weapon, bool guaranteeActivation = false)
    {
        if (!guaranteeActivation)
        {
            int invokeRoll = Random.Range(1, 101);

            if (weapon == Weapons.Sword)
            {
                if (invokeRoll <= (elementActivationChances[markings[1].markingLevel - 1] + (PlayerController.Instance.playerAttributes.Luck * 100)))
                {
                    StartCoroutine(ApplyBurn(enemiesHit, weapon));
                }
            }
            else if (weapon == Weapons.Bow)
            {
                if (invokeRoll <= (elementActivationChances[markings[3].markingLevel - 1] + (PlayerController.Instance.playerAttributes.Luck * 100)))
                {
                    StartCoroutine(ApplyBurn(enemiesHit, weapon));
                }
            }

            return;
        }

        StartCoroutine(ApplyBurn(enemiesHit, weapon));
    }

    void ApplyWindElement(List<EnemyData> enemiesHit, Weapons weapon, bool guaranteeActivation = false)
    {
        if (!guaranteeActivation)
        {
            foreach (EnemyData enemy in enemiesHit)
            {
                int invokeRoll = Random.Range(1, 101);

                if (weapon == Weapons.Sword)
                {
                    if (invokeRoll <= (elementActivationChances[markings[1].markingLevel - 1] + (PlayerController.Instance.playerAttributes.Luck * 100)))
                    {
                        enemy.GetComponentInChildren<Animator>().SetBool("Winded", true);

                        windedDuration = baseWindedDuration * elementMultipliers[markings[1].markingLevel - 1];
                    }
                }
                else if (weapon == Weapons.Bow)
                {
                    if (invokeRoll <= (elementActivationChances[markings[3].markingLevel - 1] + (PlayerController.Instance.playerAttributes.Luck * 100)))
                    {
                        enemy.GetComponentInChildren<Animator>().SetBool("Winded", true);

                        windedDuration = baseWindedDuration * elementMultipliers[markings[3].markingLevel - 1];
                    }
                }
            }

            return;
        }

        foreach (EnemyData enemy in enemiesHit)
        {
            if (weapon == Weapons.Sword)
            {
                enemy.GetComponentInChildren<Animator>().SetTrigger("Winded");

                windedDuration = baseWindedDuration * elementMultipliers[markings[1].markingLevel - 1];
            }
            else if (weapon == Weapons.Bow)
            {
                enemy.GetComponentInChildren<Animator>().SetTrigger("Winded");

                windedDuration = baseWindedDuration * elementMultipliers[markings[3].markingLevel - 1];
            }
        }
    }

    public float GetWindedDuration()
    {
        return windedDuration;
    }

    IEnumerator ApplyBurn(List<EnemyData> enemies, Weapons weapon)
    {
        for (int i = 0; i < numberOfBurnTicks; i++)
        {
            yield return new WaitForSeconds(burnTickDelay);

            foreach (EnemyData enemy in enemies)
            {
                if (weapon == Weapons.Sword)
                {
                    enemy.TakeDamage(baseBurnTickDamage * elementMultipliers[markings[1].markingLevel - 1]);
                }
                else if (weapon == Weapons.Bow)
                {
                    enemy.TakeDamage(baseBurnTickDamage * elementMultipliers[markings[3].markingLevel - 1]);
                }
            }
        }

        yield return null;
    }
}
