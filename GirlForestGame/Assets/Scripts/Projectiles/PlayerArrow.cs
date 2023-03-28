using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PlayerArrow : MonoBehaviour
{
    LayerMask livingLayer;
    LayerMask spiritLayer;
    Elements _element;
    PlayerController player;
    float arrowChargePercentage;
    float chargedArrowDamage;
    bool movementStarted;

    private void Start()
    {
        livingLayer.value = LayerMask.NameToLayer("PlayerLiving");
        spiritLayer.value = LayerMask.NameToLayer("PlayerSpirit");

        player = PlayerController.Instance;

        SetPlane(player.playerCombat.Form);
        movementStarted = false;
    }

    private void OnEnable()
    {
        livingLayer.value = LayerMask.NameToLayer("PlayerLiving");
        spiritLayer.value = LayerMask.NameToLayer("PlayerSpirit");

        player = PlayerController.Instance;

        SetPlane(player.playerCombat.Form);
        movementStarted = false;
    }

    public void SetArrowChargeMultiplier(float percentage)
    {
        //This is here because it often gets called on the same frame as start so it does not get initialized
        player = PlayerController.Instance;
        arrowChargePercentage = percentage;

        chargedArrowDamage = player.playerAttributes.BowDamage * percentage;
    }

    public void SetPlane(Planes plane)
    {
        if (plane == Planes.Terrestrial)
        {
            gameObject.layer = livingLayer.value;
        }
        else if (plane == Planes.Astral)
        {
            gameObject.layer = spiritLayer.value;
        }
    }

    public void SetMovement(Elements element, GameObject target = null)
    {
        player = PlayerController.Instance;
        _element = element;

        if (!movementStarted)
        {
            movementStarted = true;

            switch (element)
            {
                case Elements.None:

                    if (target == null)
                    {
                        transform.forward = player.aimDirection;
                        GetComponent<Rigidbody>().velocity = player.aimDirection * player.playerAttributes.ProjectileSpeed;
                    }
                    else
                    {
                        transform.LookAt(target.transform);
                        GetComponent<Rigidbody>().velocity = transform.forward * player.playerAttributes.ProjectileSpeed;
                    }

                    break;

                case Elements.Fire:

                    if (target == null)
                    {
                        transform.forward = player.aimDirection;
                        transform.rotation = transform.rotation * Quaternion.AngleAxis(-45, Vector3.right);
                        transform.DOJump(player.transform.position + (player.aimDirection * player.playerAttributes.SwordRange * 1.5f), 5, 1, 1 - ((player.playerAttributes.ProjectileSpeed * 2) / 100));
                        transform.DORotateQuaternion(transform.rotation * Quaternion.AngleAxis(135, Vector3.right), 1 - ((player.playerAttributes.ProjectileSpeed * 2) / 100));
                    }
                    else
                    {
                        transform.LookAt(target.transform);
                        transform.rotation = transform.rotation * Quaternion.AngleAxis(-45, Vector3.right);
                        transform.DOJump(new Vector3(target.transform.position.x, 0, target.transform.position.z), 5, 1, 1 - ((player.playerAttributes.ProjectileSpeed * 2) / 100));
                        transform.DORotateQuaternion(transform.rotation * Quaternion.AngleAxis(135, Vector3.right), 1 - ((player.playerAttributes.ProjectileSpeed * 2) / 100));
                    }

                    break;

                case Elements.Wind:

                    if (target == null)
                    {
                        transform.forward = player.aimDirection;
                        GetComponent<Rigidbody>().velocity = player.aimDirection * player.playerAttributes.ProjectileSpeed;
                    }
                    else
                    {
                        transform.LookAt(target.transform);
                        GetComponent<Rigidbody>().velocity = transform.forward * player.playerAttributes.ProjectileSpeed;
                    }

                    break;
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        //transform.parent = collision.gameObject.transform;

        //Checks if it hits an enemy first
        if (collision.gameObject.TryGetComponent(out EnemyData enemy) && enemy.Form == PlayerController.Instance.playerCombat.Form)
        {
            List<EnemyData> enemiesHit = new List<EnemyData>();

            enemiesHit.Add(enemy);

            EventManager.Instance.InvokeOnBowHit(enemiesHit);

            //Checks for hitting from behind for the assassin totem
            if (Vector3.Angle(transform.up, enemiesHit[0].transform.forward) < 90)
            {
                AssassinTotem t;
                bool assassinTotemExists = player.playerInventory.totemDictionary[typeof(AssassinTotem)] > 0;

                if (assassinTotemExists)
                {
                    t = (AssassinTotem)player.playerInventory.GetTotemFromList(typeof(AssassinTotem)).Totem;

                    t.SetWeaponUsed(Weapons.Bow);
                }

                enemy.ApplyKnockback(2, transform.forward);
                enemy.TakeDamage(player.playerAttributes.BowDamage + chargedArrowDamage);

                if (assassinTotemExists)
                {
                    t = (AssassinTotem)player.playerInventory.GetTotemFromList(typeof(AssassinTotem)).Totem;

                    t.RemoveEffect();
                }
            }
            else
            {
                enemy.ApplyKnockback(2, transform.forward);
                enemy.TakeDamage(player.playerAttributes.BowDamage + chargedArrowDamage);
            }

            //ProjectilePool.Instance.AddArrowToPool(gameObject);

            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial"))
            {
                TutorialManager.Instance.TriggerTutorialSection(6, true);
            }
        }

        //applies the correct effect for the element of the bow when it collides
        if (_element == Elements.Fire)
        {
            ParticleManager.Instance.SpawnParticle(ParticleTypes.FireArrow, new Vector3(transform.position.x, 0, transform.position.z), arrowChargePercentage);

            //ProjectilePool.Instance.AddArrowToPool(gameObject);
        }
        else if (_element == Elements.Wind)
        {
            ParticleManager.Instance.SpawnParticle(ParticleTypes.WindArrow, new Vector3(transform.position.x, 1, transform.position.z), arrowChargePercentage);

            //ProjectilePool.Instance.AddArrowToPool(gameObject);
        }

        ProjectilePool.Instance.AddArrowToPool(gameObject);
    }
}
