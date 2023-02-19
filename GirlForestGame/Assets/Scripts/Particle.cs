using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField] ParticleTypes particleType;
    [SerializeField] ParticleTypes childParticle = ParticleTypes.None;
    ParticleSystem particle;

    LayerMask livingLayer;
    LayerMask spiritLayer;
    LayerMask defaultLayer;

    // Start is called before the first frame update
    void Start()
    {
        livingLayer = LayerMask.NameToLayer("EnemyLiving");
        spiritLayer = LayerMask.NameToLayer("EnemySpirit");
        defaultLayer = LayerMask.NameToLayer("Default");
        particle = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        //Creates the correct layer mask for the colliders to hit the proper enemies at any given time
        int colliderLayerMask = (1 << defaultLayer);

        if (PlayerController.Instance.playerCombat.Form == Planes.Terrestrial)
        {
            gameObject.layer = livingLayer;
            colliderLayerMask |= (1 << livingLayer);
            colliderLayerMask &= ~(1 << spiritLayer);
        }
        else
        {
            gameObject.layer = spiritLayer;
            colliderLayerMask |= (1 << spiritLayer);
            colliderLayerMask &= ~(1 << livingLayer);
        }

        //If a particle has a different particle that comes after it, this will spawn it
        if (!particle.main.loop && !particle.isPlaying)
        {
            if (childParticle != ParticleTypes.None)
            {
                ParticleManager.Instance.SpawnParticle(childParticle, transform.position);
            }

            if (particleType == ParticleTypes.FearfulAura)
            {
                gameObject.SetActive(false);

                return;
            }

            Destroy(gameObject);

            return;
        }

        if (particleType == ParticleTypes.WindArrow)
        {
            Collider[] collidersInRange = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius, colliderLayerMask);

            foreach (Collider collision in collidersInRange)
            {
                if (collision.TryGetComponent(out EnemyData enemy))
                {
                    enemy.GetComponent<Rigidbody>().AddForce((transform.position - enemy.transform.position).normalized * Time.deltaTime * 5, ForceMode.VelocityChange);
                }
            }
        }
        else if (particleType == ParticleTypes.WindArrow2)
        {
            Collider[] collidersInRange = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius, colliderLayerMask);

            foreach (Collider collision in collidersInRange)
            {
                if (collision.TryGetComponent(out EnemyData enemy))
                {
                    enemy.ApplyKnockback(1, (enemy.transform.position - transform.position).normalized);

                    List<EnemyData> enemiesHit = new List<EnemyData>();

                    enemiesHit.Add(enemy);

                    EventManager.Instance.InvokeOnBowHit(enemiesHit);
                }
            }
        }
        else if (particleType == ParticleTypes.AstralBarrier)
        {
            transform.position = new Vector3 (PlayerController.Instance.transform.position.x, transform.position.y, PlayerController.Instance.transform.position.z);

            Collider[] projectilesInRange = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);

            foreach (Collider collision in projectilesInRange)
            {
                if (collision.CompareTag("EnemyProjectile"))
                {
                    Vector3 forceDirection = (collision.transform.position - transform.position).normalized;
                    forceDirection.y = 0;
                    forceDirection = forceDirection.normalized;

                    collision.GetComponent<Rigidbody>().AddForce(forceDirection * Time.deltaTime * 5, ForceMode.VelocityChange);
                }
            }
        }
        else if (particleType == ParticleTypes.FearfulAura)
        {
            transform.position = new Vector3(PlayerController.Instance.transform.position.x, transform.position.y, PlayerController.Instance.transform.position.z);

            Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);

            foreach (Collider collision in enemiesInRange)
            {
                if (collision.TryGetComponent(out EnemyData enemy))
                {
                    enemy.GetComponentInChildren<Animator>().SetTrigger("Feared");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (particleType)
        {
            case ParticleTypes.FireArrow:

                if (other.gameObject.TryGetComponent(out EnemyData enemy))
                {
                    List<EnemyData> enemiesHit = new List<EnemyData>();

                    enemiesHit.Add(enemy);

                    EventManager.Instance.InvokeOnBowHit(enemiesHit, true);
                }

                break;

            case ParticleTypes.GasCloud:

                if (other.gameObject.TryGetComponent(out PlayerCombat player))
                {
                    player.TakeDamage();
                }

                break;

            //case ParticleTypes.FearfulAura:

            //    if (other.gameObject.TryGetComponent(out EnemyData fearedEnemy))
            //    {
            //        fearedEnemy.GetComponentInChildren<Animator>().SetBool("Feared", true);
            //    }

            //    break;
        }
    }
}
