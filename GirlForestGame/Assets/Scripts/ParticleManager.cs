using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] ParticleSystem bowChargeStart;
    [SerializeField] ParticleSystem bowChargeComplete;
    [SerializeField] ParticleSystem swordSlashLeftToRight;
    [SerializeField] ParticleSystem swordSlashRightToLeft;

    ParticleSystem currentParticle;

    public static ParticleManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject SpawnParticle(ParticleTypes type, Vector3 position)
    {
        switch (type)
        {
            case ParticleTypes.DashStart:

                currentParticle = Instantiate(bowChargeStart, position, Quaternion.Euler(-90, 0, 0));

                break;

            case ParticleTypes.DashEnd:

                currentParticle = Instantiate(bowChargeComplete, position, Quaternion.Euler(-90, 0, 0));

                break;

            case ParticleTypes.SwordSlashLR:

                currentParticle = Instantiate(swordSlashLeftToRight, position, Quaternion.Euler(-90, 0, 0));

                break;

            case ParticleTypes.SwordSlashRL:

                currentParticle = Instantiate(swordSlashRightToLeft, position, Quaternion.Euler(-90, 0, 0));

                break;
        }

        return currentParticle.gameObject;
    }
}
