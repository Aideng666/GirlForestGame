using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// EnemyData is the parent class to all enemies in the game.
/// Any function that will apply to all enemies can be put here (e.g. Health)
/// </summary>
public class EnemyData : MonoBehaviour
{
    [HideInInspector]
    public float curHealth;
    [SerializeField] EnemyTypes enemyType;
    public float maxHealth = 1f;

    //Cooldown between each attack from the condition
    public float actionCooldown = 0f;

    public float enemyMaxSpeed { get; private set; } = 5;
    [SerializeField] protected Planes form;
    [SerializeField] float defaultCoinDropChance = 0.25f;
    [SerializeField] float defaultHealthDropChance = 0.05f;
    [SerializeField] Material damageTakenMat;

    [SerializeField] ParticleSystem fireEffect;
    [SerializeField] ParticleSystem windedEffect;
    [SerializeField] ParticleSystem fearedEffect;
    
    float damageShaderDuration = 0.4f;
    bool damageShaderApplied = false;

    Material originalMat;
    Shader originalShader;

    //reference to navmesh for knockback
    public NavMeshAgent agent { get; private set; }

    Rigidbody body;
    PlayerController player;
    bool isDead;
    bool isAttacking;

    float[] statMultiplierPerLevel = new float[3] { 1, 1.5f, 2 };

    public bool IsAttacking { get { return isAttacking; } set { isAttacking = value; } }

    public Planes Form { get { return form; } }

    private void OnEnable()
    {
        curHealth = maxHealth * statMultiplierPerLevel[NodeMapManager.Instance.GetCurrentMapCycle() - 1];
        isDead = false;

        fireEffect.Stop();
        windedEffect.Stop();
        fearedEffect.Stop();
    }

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        player = PlayerController.Instance;

        statMultiplierPerLevel = new float[3] { 1, 1.5f, 2 };
    }

    void Update() 
    {
        //RunCooldownTimer();
        if (!isDead)
        {
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        }
    }

    public void EnemyDeath() 
    {
        //Check if the enemy is part of the tutorial, if so, make sure it cant die until it is supposed to
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial"))
        {
            if (TutorialManager.Instance.currentDialogueNum < 9)
            {
                return;
            }
        }

        isDead = true;

        //Children are supposed to override so that they can have unique death events
        GetComponentInChildren<Animator>().SetTrigger("Is_Dead");

        if (player.playerInventory.totemDictionary[typeof(VampireBiteTotem)] > 0)
        {
            player.playerInventory.GetTotemFromList(typeof(VampireBiteTotem)).Totem.ApplyEffect();
        }

        player.playerCombat.RemoveSwordTarget(this);
        player.playerCombat.RemoveBowTarget(this);
        EnemyPool.Instance.AddEnemyToPool(enemyType, gameObject);

        float coinRoll = Random.Range(0f, 1f);
        float heartRoll = Random.Range(0f, 1f);

        if (coinRoll < defaultCoinDropChance + player.playerAttributes.Luck)
        {
            GameObject coin = PickupPool.Instance.GetCoinFromPool(transform.position);

            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main"))
            {
                coin.transform.parent = DungeonGenerator.Instance.GetCurrentRoom().transform;
            }
        }

        if (heartRoll < defaultHealthDropChance + (player.playerAttributes.Luck / 2))
        {
            int heartToDrop = Random.Range(0, 2);

            if (heartToDrop == 0)
            {
                GameObject heart = PickupPool.Instance.GetHalfHeartFromPool(transform.position);
                heart.transform.parent = DungeonGenerator.Instance.GetCurrentRoom().transform;
            }
            if (heartToDrop == 1)
            {
                GameObject heart = PickupPool.Instance.GetHeartFromPool(transform.position);
                heart.transform.parent = DungeonGenerator.Instance.GetCurrentRoom().transform;
            }
        }

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial"))
        {
            TutorialManager.Instance.TriggerTutorialSection(10, true);
        }
    }

    /// <summary>
    ///  Take Damage applies damage and, if second argument is true, gives knockback based on the damage (using default direction)
    /// </summary>
    public void TakeDamage(float damageAmount) //Take Damage applies damage and gives knockback based on the damage, unless false
    {
        if (!isDead)
        {
            //For Mushroom Spirit activation if it gets hit before the enemy gets close
            if (enemyType == EnemyTypes.MushroomSpirit)
            {
                GetComponentInChildren<Animator>().SetTrigger("Awaken_From_Idle");
            }

            curHealth -= damageAmount;

            if (curHealth <= 0)
            {
                EnemyDeath();

                return;
            }

            StartCoroutine(ApplyDamageShader());
        }
    }

    IEnumerator ApplyDamageShader()
    {
        if (!damageShaderApplied)
        {
            damageShaderApplied = true;

            //originalShader = GetComponentInChildren<SkinnedMeshRenderer>().material.shader;
            originalMat = GetComponent<MeshRenderer>().material;

            GetComponentInChildren<MeshRenderer>().material = damageTakenMat;

            yield return new WaitForSeconds(damageShaderDuration);

            GetComponentInChildren<MeshRenderer>().material = originalMat;

            damageShaderApplied = false;
        }
        else
        {
            GetComponentInChildren<MeshRenderer>().material = damageTakenMat;

            yield return new WaitForSeconds(damageShaderDuration);

            GetComponentInChildren<MeshRenderer>().material = originalMat;

            damageShaderApplied = false;
        }
    }

    public void ApplyKnockback(float knockBack, Vector3 direction = default(Vector3))
    {
        if (enemyType == EnemyTypes.StoneGolem)
        {
            return;
        }

        body.velocity = Vector3.zero;
        GetComponentInChildren<Animator>().SetTrigger("Is_Hit");

        if (direction == Vector3.zero)
        {
            direction = transform.position - PlayerController.Instance.transform.position;
        }

        direction.y = 0f;
        body.AddForce(direction.normalized * knockBack, ForceMode.VelocityChange);
    }

    public void RunCooldownTimer()
    {
        if (actionCooldown > 0)
        {
            actionCooldown -= Time.deltaTime;
        }
    }

    public void ActivateFireParticle()
    {
        fireEffect.Play();
    }

    public void DeactivateFireParticle()
    {
        fireEffect.Stop();
    }

    public void ActivateWindParticle()
    {
        windedEffect.Play();
    }

    public void DeactivateWindParticle()
    {
        windedEffect.Stop();
    }

    public bool FireParticleActive()
    {
        return fireEffect.isEmitting;
    }

    public bool WindedParticleActive()
    {
        return windedEffect.isEmitting;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        //Vector3 directionVector = transform.position - player.transform.position;
        //directionVector.y = 0;
        //directionVector = directionVector.normalized;

        //Gizmos.DrawRay(new Vector3(player.transform.position.x, 0, player.transform.position.z), directionVector * 12);
    }
}
