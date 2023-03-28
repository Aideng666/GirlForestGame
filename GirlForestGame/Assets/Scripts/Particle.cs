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

    [HideInInspector] public FMOD.Studio.EventInstance FireSFX;
    [HideInInspector] public FMOD.Studio.EventInstance WindSFX;
    [HideInInspector] public FMOD.Studio.EventInstance WindBlastSFX;

    // Start is called before the first frame update
    void Start()
    {
        livingLayer = LayerMask.NameToLayer("EnemyLiving");
        spiritLayer = LayerMask.NameToLayer("EnemySpirit");
        defaultLayer = LayerMask.NameToLayer("Default");

        FireSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Bow/Fire");
        WindSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Bow/Wind");
        WindBlastSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Bow/WindBlast");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(FireSFX, transform);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(WindSFX, transform);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(WindBlastSFX, transform);

        particle = GetComponent<ParticleSystem>();
        Debug.Log(particleType);
        if (particleType == ParticleTypes.WindArrow)
        {
            WindSFX.start();
            Debug.Log("windsounded");
        }
        else if (particleType == ParticleTypes.FireArrow)
        {
            FireSFX.start();
            Debug.Log("firesounded");
        }
    }


    // Update is called once per frame
    void Update()
    {
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

        if (!particle.isPlaying)
        {
            //FireSFX.keyOff();
            FireSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Debug.Log("fireOFF");
            FireSFX.release();
            //WindBlastSFX.release();

            if (childParticle != ParticleTypes.None)
            {
                ParticleManager.Instance.SpawnParticle(childParticle, transform.position);
                WindSFX.keyOff();
                WindSFX.release();
                WindBlastSFX.start();
                WindBlastSFX.release();
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
    private void OnDestroy()
    {
        FireSFX.keyOff();
        FireSFX.release();
        WindBlastSFX.release();
    }
}
