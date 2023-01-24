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

    // Start is called before the first frame update
    void Start()
    {
        livingLayer = LayerMask.NameToLayer("PlayerLiving");
        spiritLayer = LayerMask.NameToLayer("PlayerSpirit");
        particle = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.layer = PlayerController.Instance.gameObject.layer;

        if (!particle.isPlaying)
        {
            if (childParticle != ParticleTypes.None)
            {
                ParticleManager.Instance.SpawnParticle(childParticle, transform.position);
            }

            Destroy(gameObject);

            return;
        }

        if (particleType == ParticleTypes.WindArrow)
        {
            Collider[] collidersInRange = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);

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
            Collider[] collidersInRange = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);

            foreach (Collider collision in collidersInRange)
            {
                if (collision.TryGetComponent(out EnemyData enemy))
                {
                    //enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    //enemy.GetComponent<Rigidbody>().AddForce((enemy.transform.position - transform.position).normalized * 5, ForceMode.Impulse);
                    enemy.ApplyKnockback(1, (enemy.transform.position - transform.position).normalized);

                    List<EnemyData> enemiesHit = new List<EnemyData>();

                    enemiesHit.Add(enemy);

                    EventManager.Instance.InvokeOnBowHit(enemiesHit);
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
        }
    }
}
