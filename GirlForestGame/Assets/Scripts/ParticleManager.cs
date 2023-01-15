using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] ParticleSystem bowChargeStart;
    [SerializeField] ParticleSystem bowChargeComplete;
    [SerializeField] ParticleSystem swordSlashLeftToRight;
    [SerializeField] ParticleSystem swordSlashRightToLeft;
    [SerializeField] ParticleSystem fireArrow;

    ParticleSystem currentParticle;

    public static ParticleManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject SpawnParticle(ParticleTypes type, Vector3 position, float percentageOfAddedRadius = 0)
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

            case ParticleTypes.FireArrow:

                var particle = Instantiate(fireArrow, position, Quaternion.Euler(-90, 0, 0));

                var particleShape = particle.GetComponent<ParticleSystem>().shape;

                particleShape.radius += particleShape.radius * percentageOfAddedRadius;

                currentParticle = particle;

                break;
        }

        return currentParticle.gameObject;
    }
}
