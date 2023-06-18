using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SG_Rock : MonoBehaviour
{
    bool isBigRock;

    //[HideInInspector] public FMOD.Studio.EventInstance crashSFX;

    // Start is called before the first frame update
    //void Start()
    //{
    //    crashSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Enemy/Golem/Crash");
    //}

    private void OnDisable()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x >= 1)
        {
            isBigRock = true;
        }
        else
        {
            isBigRock = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PassiveCollider"))
        {
            return;
        }

        if (isBigRock)
        {
            RockBurst();
            FMODUnity.RuntimeManager.PlayOneShot("event:/Enemy/Golem/Crash", collision.transform.position);
            //Debug.Log("bigrockboom");
        }

        if (gameObject.layer == LayerMask.NameToLayer("EnemyLiving"))
        {
            ProjectilePool.Instance.AddProjectileToPool(Planes.Terrestrial, gameObject, EnemyTypes.StoneGolem);
        }
        else if (gameObject.layer == LayerMask.NameToLayer("EnemySpirit"))
        {
            ProjectilePool.Instance.AddProjectileToPool(Planes.Astral, gameObject, EnemyTypes.StoneGolem);
        }


    }

    void RockBurst()
    {

        if (gameObject.layer == LayerMask.NameToLayer("EnemyLiving"))
        {
            GameObject rock1 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Astral, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock1.transform.localScale /= 2;
            rock1.GetComponent<Rigidbody>().AddForce((Vector3.up + Vector3.forward * 2).normalized * 7, ForceMode.VelocityChange);

            GameObject rock2 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Astral, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock2.transform.localScale /= 2;
            rock2.GetComponent<Rigidbody>().AddForce((Vector3.up + Vector3.back * 2).normalized * 7, ForceMode.VelocityChange);

            GameObject rock3 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Astral, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock3.transform.localScale /= 2;
            rock3.GetComponent<Rigidbody>().AddForce((Vector3.up + Vector3.left * 2).normalized * 7, ForceMode.VelocityChange);

            GameObject rock4 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Astral, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock4.transform.localScale /= 2;
            rock4.GetComponent<Rigidbody>().AddForce((Vector3.up + Vector3.right * 2).normalized * 7, ForceMode.VelocityChange);

            GameObject rock5 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Astral, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock5.transform.localScale /= 2;
            rock5.GetComponent<Rigidbody>().AddForce((Vector3.up + (Vector3.forward * 2) + (Vector3.left * 2)).normalized * 7, ForceMode.VelocityChange);

            GameObject rock6 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Astral, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock6.transform.localScale /= 2;
            rock6.GetComponent<Rigidbody>().AddForce((Vector3.up + (Vector3.forward * 2) + (Vector3.right * 2)).normalized * 7, ForceMode.VelocityChange);

            GameObject rock7 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Astral, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock7.transform.localScale /= 2;
            rock7.GetComponent<Rigidbody>().AddForce((Vector3.up + (Vector3.back * 2) + (Vector3.left * 2)).normalized * 7, ForceMode.VelocityChange);

            GameObject rock8 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Astral, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock8.transform.localScale /= 2;
            rock8.GetComponent<Rigidbody>().AddForce((Vector3.up + (Vector3.back * 2) + (Vector3.right * 2)).normalized * 7, ForceMode.VelocityChange);
        }
        else if (gameObject.layer == LayerMask.NameToLayer("EnemySpirit"))
        {
            GameObject rock1 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Terrestrial, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock1.transform.localScale /= 2;
            rock1.GetComponent<Rigidbody>().AddForce((Vector3.up + Vector3.forward * 2).normalized * 7, ForceMode.VelocityChange);

            GameObject rock2 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Terrestrial, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock2.transform.localScale /= 2;
            rock2.GetComponent<Rigidbody>().AddForce((Vector3.up + Vector3.back * 2).normalized * 7, ForceMode.VelocityChange);

            GameObject rock3 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Terrestrial, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock3.transform.localScale /= 2;
            rock3.GetComponent<Rigidbody>().AddForce((Vector3.up + Vector3.left * 2).normalized * 7, ForceMode.VelocityChange);

            GameObject rock4 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Terrestrial, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock4.transform.localScale /= 2;
            rock4.GetComponent<Rigidbody>().AddForce((Vector3.up + Vector3.right * 2).normalized * 7, ForceMode.VelocityChange);

            GameObject rock5 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Terrestrial, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock5.transform.localScale /= 2;
            rock5.GetComponent<Rigidbody>().AddForce((Vector3.up + (Vector3.forward * 2) + (Vector3.left * 2)).normalized * 7, ForceMode.VelocityChange);

            GameObject rock6 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Terrestrial, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock6.transform.localScale /= 2;
            rock6.GetComponent<Rigidbody>().AddForce((Vector3.up + (Vector3.forward * 2) + (Vector3.right * 2)).normalized * 7, ForceMode.VelocityChange);

            GameObject rock7 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Terrestrial, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock7.transform.localScale /= 2;
            rock7.GetComponent<Rigidbody>().AddForce((Vector3.up + (Vector3.back * 2) + (Vector3.left * 2)).normalized * 7, ForceMode.VelocityChange);

            GameObject rock8 = ProjectilePool.Instance.GetProjectileFromPool(Planes.Terrestrial, transform.position + Vector3.up, EnemyTypes.StoneGolem);
            rock8.transform.localScale /= 2;
            rock8.GetComponent<Rigidbody>().AddForce((Vector3.up + (Vector3.back * 2) + (Vector3.right * 2)).normalized * 7, ForceMode.VelocityChange);
        }
    }
    //private void OnDestroy()
    //{
    //    crashSFX.release();
    //}
}
