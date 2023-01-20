using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    [SerializeField] ParticleTypes particleType;
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
            Destroy(gameObject);
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

                    print("Enemy walked into fire");
                }

                break;
        }
    }
}
