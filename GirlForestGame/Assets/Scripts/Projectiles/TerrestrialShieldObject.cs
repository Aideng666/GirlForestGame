using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrestrialShieldObject : MonoBehaviour
{

    bool cooldownApplied = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = PlayerController.Instance.transform.position + Vector3.up;
        transform.GetChild(0).RotateAround(transform.position, Vector3.up, 0.2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyProjectile"))
        {
            ParticleManager.Instance.SpawnParticle(ParticleTypes.ShieldCrumble, transform.position);

            FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Totem/Shieldbreak");


            cooldownApplied = true;
            gameObject.SetActive(false);
        }
    }

    public bool GetCooldownApplied()
    {
        return cooldownApplied;
    }

    public void SetCooldownApplied(bool applied)
    {
        cooldownApplied = applied;
    }
}
