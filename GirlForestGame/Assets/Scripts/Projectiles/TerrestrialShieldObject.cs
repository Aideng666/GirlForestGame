using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrestrialShieldObject : MonoBehaviour
{
    [HideInInspector] public FMOD.Studio.EventInstance sBreakSFX;

    bool cooldownApplied = false;
    private void OnEnable()
    {
        sBreakSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Totem/Shieldbreak");
    }

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
           // sBreakSFX.start();
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
