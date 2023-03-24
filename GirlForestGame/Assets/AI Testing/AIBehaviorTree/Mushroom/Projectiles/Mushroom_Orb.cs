using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_Orb : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {

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
