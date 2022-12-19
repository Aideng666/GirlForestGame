using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMarkings : MonoBehaviour
{
    [SerializeField] int numberOfBurnTicks = 5;
    [SerializeField] int burnTickDamage = 1;
    [SerializeField] float burnTickDelay = 1;

    //to access the player's scripts
    //The player controller contains access to the other player scripts
    PlayerController player;

    //Spirit bowAttribute = null;
    //Spirit swordAttribute = null;
    //Spirit bowElement = null;
    //Spirit swordElement = null;

    //public Spirit BowAttribute { get { return bowAttribute; } set { bowAttribute = value; } }
    //public Spirit SwordAttribute { get { return swordAttribute; } set { swordAttribute = value; } }
    //public Spirit BowElement { get { return bowElement; } set { bowElement = value; } }
    //public Spirit SwordElement { get { return swordElement; } set { swordElement = value; } }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();
    }

    private void OnDisable()
    {
        PlayerController.OnSwordHit -= ApplyWindElement;
        PlayerController.OnSwordHit -= ApplyFireElement;
        PlayerArrow.OnBowHit -= ApplyFireElement;
        PlayerArrow.OnBowHit -= ApplyWindElement;
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
        if (type == MarkingTypes.Attribute)
        {
            for (int i = 0; i < spirit.buffedAttributes.Count; i++)
            {
                switch (spirit.buffedAttributes[i])
                {
                    //Buff all of these based on the level of the marking when adding level functionality - This is for Aiden dw abt it
                    case Attributes.Health:

                        player.playerAttributes.MaxHealth += 2;

                        break;

                    case Attributes.Attack:

                        if (weapon == Weapons.Sword)
                        {
                            player.playerAttributes.SwordDamage *= 1.75f;
                        }
                        else if (weapon == Weapons.Bow)
                        {
                            player.playerAttributes.BowDamage *= 1.75f;
                        }

                        break;

                    case Attributes.AtkSpd:

                        if (weapon == Weapons.Sword)
                        {
                            player.playerAttributes.SwordCooldown /= 1.75f;
                        }
                        else if (weapon == Weapons.Bow)
                        {
                            player.playerAttributes.BowCooldown /= 1.75f;
                        }

                        break;

                    case Attributes.Speed:

                        player.playerAttributes.Speed *= 1.75f;

                        break;

                    case Attributes.Accuracy:

                        player.playerAttributes.ProjectileSpeed *= 1.75f;

                        break;

                    case Attributes.CritChance:

                        player.playerAttributes.CritChance *= 1.75f;

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
                        PlayerController.OnSwordHit += ApplyWindElement;
                    }
                    else if (weapon == Weapons.Bow)
                    {
                        PlayerArrow.OnBowHit += ApplyWindElement;
                    }

                    break;

                case Elements.Fire:

                    if (weapon == Weapons.Sword)
                    {
                        PlayerController.OnSwordHit += ApplyFireElement;
                    }
                    else if (weapon == Weapons.Bow)
                    {
                        PlayerArrow.OnBowHit += ApplyFireElement;
                    }

                    break;
            }
        }
    }

    public void RemoveMarking(Spirit spirit, MarkingTypes type, Weapons weapon)
    {
        if (type == MarkingTypes.Attribute)
        {
            print($"Removing {spirit.spiritName} {type.ToString()}");

            for (int i = 0; i < spirit.buffedAttributes.Count; i++)
            {
                switch (spirit.buffedAttributes[i])
                {
                    case Attributes.Health:

                        player.playerAttributes.MaxHealth -= 2;

                        break;

                    case Attributes.Attack:

                        if (weapon == Weapons.Sword)
                        {
                            player.playerAttributes.SwordDamage /= 1.75f;
                        }
                        else if (weapon == Weapons.Bow)
                        {
                            player.playerAttributes.BowDamage /= 1.75f;
                        }

                        break;

                    case Attributes.AtkSpd:

                        if (weapon == Weapons.Sword)
                        {
                            player.playerAttributes.SwordCooldown *= 1.75f;
                        }
                        else if (weapon == Weapons.Bow)
                        {
                            player.playerAttributes.BowCooldown *= 1.75f;
                        }

                        break;

                    case Attributes.Speed:

                        player.playerAttributes.Speed /= 1.75f;

                        break;

                    case Attributes.Accuracy:

                        player.playerAttributes.ProjectileSpeed /= 1.75f;

                        break;

                    case Attributes.CritChance:

                        player.playerAttributes.CritChance /= 1.75f;

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
                        PlayerController.OnSwordHit -= ApplyWindElement;
                    }
                    else if (weapon == Weapons.Bow)
                    {
                        PlayerArrow.OnBowHit -= ApplyWindElement;
                    }

                    break;

                case Elements.Fire:

                    if (weapon == Weapons.Sword)
                    {
                        PlayerController.OnSwordHit -= ApplyFireElement;
                    }
                    else if (weapon == Weapons.Bow)
                    {
                        PlayerArrow.OnBowHit -= ApplyFireElement;
                    }

                    break;
            }
        }
    }

    void ApplyFireElement(List<Enemy> enemiesHit)
    {
        print("FIRE");

        StartCoroutine(ApplyBurn(enemiesHit));
    }

    void ApplyWindElement(List<Enemy> enemiesHit)
    {
        print("Wind");

        foreach(Enemy enemy in enemiesHit)
        {
            enemy.ApplyKnockback(player.transform.forward, 10);
        }
    }

    IEnumerator ApplyBurn(List<Enemy> enemies)
    {
        for (int i = 0; i < numberOfBurnTicks; i++)
        {
            yield return new WaitForSeconds(burnTickDelay);

            foreach (Enemy enemy in enemies)
            {
                enemy.TakeDamage(burnTickDamage);
            }
        }

        yield return null;
    }

    //IEnumerator SelectWeapon(Spirit spirit, MarkingTypes type)
    //{
    //    bool weaponSelected = false;

    //    while (!weaponSelected)
    //    {
    //        if (InputManager.Instance.SelectSword())
    //        {
    //            if (type == MarkingTypes.Attribute)
    //            {
    //                //CHECK IF THE PLAYER ALREADY HAS A SPIRIT IN EACH SLOT TO BE ABLE TO SWAP - This is for aiden dont worry abt it
    //                SwordAttribute = spirit;
    //            }
    //            else if (type == MarkingTypes.Element)
    //            {
    //                SwordElement = spirit;
    //            }

    //            UpdateMarking(spirit, type, Weapons.Sword);

    //            weaponSelected = true;
    //        }
    //        if (InputManager.Instance.SelectBow())
    //        {
    //            if (type == MarkingTypes.Attribute)
    //            {
    //                BowAttribute = spirit;
    //            }
    //            else if (type == MarkingTypes.Element)
    //            {
    //                BowElement = spirit;
    //            }

    //            UpdateMarking(spirit, type, Weapons.Bow);

    //            weaponSelected = true;
    //        }

    //        yield return null;
    //    }
    //}
}
