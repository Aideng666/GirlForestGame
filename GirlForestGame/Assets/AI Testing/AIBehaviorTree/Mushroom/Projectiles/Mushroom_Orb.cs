using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_Orb : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.TryGetComponent(out PlayerController player))
        //{
        //    player.playerCombat.ApplyKnockback((player.transform.position - transform.position).normalized, 20);
        //    player.playerCombat.TakeDamage();
        //}

        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            return;
        }

        if (gameObject.layer == LayerMask.NameToLayer("EnemyLiving"))
        {
            ProjectilePool.Instance.AddProjectileToPool(Planes.Terrestrial, gameObject, EnemyTypes.MushroomSpirit);
        }
        else if (gameObject.layer == LayerMask.NameToLayer("EnemySpirit"))
        {
            ProjectilePool.Instance.AddProjectileToPool(Planes.Astral, gameObject, EnemyTypes.MushroomSpirit);
        }
    }
}
