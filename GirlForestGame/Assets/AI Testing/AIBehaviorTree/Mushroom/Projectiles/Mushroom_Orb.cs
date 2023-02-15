using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom_Orb : MonoBehaviour
{
    //[SerializeField] int collisionLayer;
    //private void OnTriggerEnter(Collider collision)
    //{
    //    //TODO: update this with the correct layer
    //    if (collision.gameObject.layer == this.gameObject.layer - 2) //This is attorcious, but with the current layout it will work. 
    //    {
    //        //Damage

    //        //TODO: Don't destroy, use object pool
    //        Destroy(this.gameObject);
    //    }
    //    else if (collision.gameObject.CompareTag("Environment")) 
    //    {
    //        //TODO: Don't destroy, use object pool
    //        Destroy(this.gameObject);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerController player))
        {
            player.playerCombat.ApplyKnockback((player.transform.position - transform.position).normalized, 20);
            player.playerCombat.TakeDamage();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            return;
        }

        if (gameObject.layer == LayerMask.NameToLayer("EnemyLiving"))
        {
            ProjectilePool.Instance.AddProjectileToPool(Planes.Terrestrial, gameObject);
        }
        else if (gameObject.layer == LayerMask.NameToLayer("EnemySpirit"))
        {
            ProjectilePool.Instance.AddProjectileToPool(Planes.Astral, gameObject);
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.TryGetComponent(out PlayerController player))
    //    {
    //        player.playerCombat.ApplyKnockback((player.transform.position - transform.position).normalized, 20);
    //        player.playerCombat.TakeDamage();
    //    }

    //    print($"Collided With {collision.gameObject.name}");


    //    if (gameObject.layer == LayerMask.NameToLayer("EnemyLiving"))
    //    {
    //        ProjectilePool.Instance.AddProjectileToPool(Planes.Terrestrial, gameObject);
    //    }
    //    else if (gameObject.layer == LayerMask.NameToLayer("EnemySpirit"))
    //    {
    //        ProjectilePool.Instance.AddProjectileToPool(Planes.Astral, gameObject);
    //    }
    //}
}
