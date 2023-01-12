using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    LayerMask livingLayer;
    LayerMask spiritLayer;
    float chargedArrowDamage;

    private void Start()
    {
        print(LayerMask.NameToLayer("PlayerLiving"));

        livingLayer.value = LayerMask.NameToLayer("PlayerLiving");
        spiritLayer.value = LayerMask.NameToLayer("PlayerSpirit");

        SetPlane(PlayerController.Instance.playerCombat.Form);
    }

    public void SetArrowChargeMultiplier(float percentage)
    {
        chargedArrowDamage = PlayerController.Instance.playerAttributes.BowDamage * percentage;
    }

    public void SetPlane(Forms plane)
    {
        if (plane == Forms.Living)
        {
            gameObject.layer = livingLayer.value;
        }
        else if (plane == Forms.Spirit)
        {
            gameObject.layer = spiritLayer.value;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        PlayerController player = PlayerController.Instance;

        transform.parent = collision.gameObject.transform;

        if (collision.gameObject.TryGetComponent(out EnemyData enemy) && enemy.Form == PlayerController.Instance.playerCombat.Form)
        {
            List<EnemyData> enemiesHit = new List<EnemyData>();

            enemiesHit.Add(enemy);

            EventManager.Instance.InvokeOnBowHit(enemiesHit);

            if (Vector3.Angle(transform.up, enemiesHit[0].transform.forward) < 90)
            {
                AssassinTotem t;
                bool assassinTotemExists = player.playerInventory.totemDictionary[typeof(AssassinTotem)] > 0;

                if (assassinTotemExists)
                {
                    t = (AssassinTotem)player.playerInventory.GetTotemFromList(typeof(AssassinTotem)).Totem;

                    t.SetWeaponUsed(Weapons.Bow);

                    print("Increased Damage");
                }

                enemy.ApplyKnockback(2, transform.forward);
                enemy.TakeDamage(player.playerAttributes.BowDamage + chargedArrowDamage);

                print($"Enemy Took {player.playerAttributes.BowDamage + chargedArrowDamage}");

                if (assassinTotemExists)
                {
                    t = (AssassinTotem)player.playerInventory.GetTotemFromList(typeof(AssassinTotem)).Totem;

                    t.RemoveEffect();

                    print("Lowered Damage");
                }
            }
            else
            {
                enemy.ApplyKnockback(2, transform.forward);
                enemy.TakeDamage(player.playerAttributes.BowDamage + chargedArrowDamage);
            }

            Destroy(gameObject);
        }
    }
}
