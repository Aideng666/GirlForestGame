using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// EnemyData is the parent class to all enemies in the game.
/// Any function that will apply to all enemies can be put here (e.g. Health)
/// </summary>
public class EnemyData : MonoBehaviour
{
    [HideInInspector]
    public float curHealth;
    public float maxHealth = 1f;

    //Cooldown between each attack from the condition
    public float actionCooldown = 0f;

    public float enemyMaxSpeed { get; private set; } = 5;
    [SerializeField] float defaultCoinDropChance = 0.25f;
    [SerializeField] float defaultHealthDropChance = 0.05f;

    //reference to navmesh for knockback
    public NavMeshAgent agent { get; private set; }

    Rigidbody body;
    PlayerController player;
    bool isDead;

    //CHANGE THIS TO BE MORE FLEXIBLE
    protected Planes form = Planes.Terrestrial;
    public Planes Form { get { return form; } }

    private void OnEnable()
    {
        curHealth = maxHealth;
        isDead = false;
    }

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        player = PlayerController.Instance;
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
        isDead = true;

        //Children are supposed to override so that they can have unique death events
        GetComponentInChildren<Animator>().SetTrigger("Is_Dead");

        if (player.playerInventory.totemDictionary[typeof(VampireBiteTotem)] > 0)
        {
            player.playerInventory.GetTotemFromList(typeof(VampireBiteTotem)).Totem.ApplyEffect();
        }

        player.playerCombat.RemoveSwordTarget(this);
        player.playerCombat.RemoveBowTarget(this);
        EnemyPool.Instance.AddBoarToPool(gameObject);

        float coinRoll = Random.Range(0f, 1f);
        float heartRoll = Random.Range(0f, 1f);

        if (coinRoll < defaultCoinDropChance + player.playerAttributes.Luck)
        {
            GameObject coin = PickupPool.Instance.GetCoinFromPool(transform.position);
            coin.transform.parent = DungeonGenerator.Instance.GetCurrentRoom().transform;
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
    }

    /// <summary>
    ///  Take Damage applies damage and, if second argument is true, gives knockback based on the damage (using default direction)
    /// </summary>
    public void TakeDamage(float damageAmount) //Take Damage applies damage and gives knockback based on the damage, unless false
    {
        if (!isDead)
        {
            curHealth -= damageAmount;

            if (curHealth <= 0)
            {
                EnemyDeath();
            }
        }
    }

    public void ApplyKnockback(float knockBack, Vector3 direction = default(Vector3))
    {
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
}
