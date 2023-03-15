using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.ParticleSystem;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] ParticleSystem bowChargeStart;
    [SerializeField] ParticleSystem bowChargeComplete;
    [SerializeField] ParticleSystem swordSlashLeftToRight;
    [SerializeField] ParticleSystem swordSlashRightToLeft;
    [SerializeField] ParticleSystem fireArrow;
    [SerializeField] ParticleSystem windArrow;
    [SerializeField] ParticleSystem windArrow2;
    [SerializeField] ParticleSystem gasCloud;
    [SerializeField] ParticleSystem astralBarrier;
    [SerializeField] ParticleSystem fearfulAura;
    [SerializeField] ParticleSystem teleport;
    [SerializeField] ParticleSystem teleportAttack;
    [SerializeField] ParticleSystem shieldCrumble;

    ParticleSystem currentParticle;

    ShapeModule windParticleShape;

    public static ParticleManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject SpawnParticle(ParticleTypes type, Vector3 position, float percentageOfAddedRadius = 0)
    {
        switch (type)
        {
            case ParticleTypes.BowCharge:

                currentParticle = Instantiate(bowChargeStart, position, Quaternion.Euler(-90, 0, 0));

                break;

            case ParticleTypes.BowCharge2:

                currentParticle = Instantiate(bowChargeComplete, position, Quaternion.Euler(-90, 0, 0));

                break;

            case ParticleTypes.SwordSlashLR:

                currentParticle = Instantiate(swordSlashLeftToRight, position, Quaternion.Euler(-90, 0, 0));

                break;

            case ParticleTypes.SwordSlashRL:

                currentParticle = Instantiate(swordSlashRightToLeft, position, Quaternion.Euler(-90, 0, 0));

                break;

            case ParticleTypes.FireArrow:

                var fireParticle = Instantiate(fireArrow, position, Quaternion.Euler(-90, 0, 0));

                var fireParticleShape = fireParticle.GetComponent<ParticleSystem>().shape;

                fireParticleShape.radius += fireParticleShape.radius * percentageOfAddedRadius;

                currentParticle = fireParticle;

                currentParticle.GetComponent<SphereCollider>().radius = fireParticleShape.radius;

                break;

            case ParticleTypes.WindArrow:

                var windParticle = Instantiate(windArrow, position, Quaternion.Euler(-90, 0, 0));

                windParticleShape = windParticle.GetComponent<ParticleSystem>().shape;

                windParticleShape.radius += windParticleShape.radius * percentageOfAddedRadius;

                currentParticle = windParticle;

                currentParticle.GetComponent<SphereCollider>().radius = windParticleShape.radius;

                break;

            case ParticleTypes.WindArrow2:

                currentParticle = Instantiate(windArrow2, position, Quaternion.Euler(-90, 0, 0));

                currentParticle.GetComponent<SphereCollider>().radius = windParticleShape.radius;

                break;

            case ParticleTypes.GasCloud:

                currentParticle = Instantiate(gasCloud, position, gasCloud.gameObject.transform.rotation);

                break;

            case ParticleTypes.AstralBarrier:

                currentParticle = Instantiate(astralBarrier, new Vector3(position.x, astralBarrier.transform.position.y, position.z), astralBarrier.gameObject.transform.rotation, PlayerController.Instance.transform);

                break;

            case ParticleTypes.FearfulAura:

                currentParticle = Instantiate(fearfulAura, new Vector3(position.x, fearfulAura.transform.position.y, position.z), fearfulAura.gameObject.transform.rotation, PlayerController.Instance.transform);

                break;

            case ParticleTypes.Teleport:

                currentParticle = Instantiate(teleport, new Vector3(position.x, teleport.transform.position.y, position.z), teleport.gameObject.transform.rotation);

                break;

            case ParticleTypes.TeleportAttack:

                currentParticle = Instantiate(teleportAttack, new Vector3(position.x, teleportAttack.transform.position.y, position.z), teleportAttack.gameObject.transform.rotation);

                break;

            case ParticleTypes.ShieldCrumble:

                currentParticle = Instantiate(shieldCrumble, new Vector3(position.x, shieldCrumble.transform.position.y, position.z), shieldCrumble.gameObject.transform.rotation);

                break;
        }

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main"))
        {
            currentParticle.transform.parent = DungeonGenerator.Instance.GetCurrentRoom().transform;
        }

        return currentParticle.gameObject;
    }
}
