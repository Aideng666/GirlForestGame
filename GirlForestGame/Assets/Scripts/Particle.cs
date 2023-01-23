using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField] ParticleTypes particleType;
    [SerializeField] ParticleTypes childParticle = ParticleTypes.None;
    ParticleSystem particle;

    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
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
            Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, GetComponent<ParticleSystem>().shape.radius);
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

                    EventManager.Instance.InvokeOnBowHit(enemiesHit);
                }

                break;
        }
    }
}
