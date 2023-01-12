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

    //May not be needed
    //public float weight = 1f;

    //What damage an attack does. Down the line this can become a list for the different attacks the AI can have
    //public float attackDamage = 1f;

    float wallOffsetToSpawn = 5;
    bool enemyInitialized = false; //This is a flag to be able to spawn enemies only when you walk in the room

    //reference to navmesh for knockback
  //  NavMeshAgent mesh;

    //CHANGE THIS TO BE MORE FLEXIBLE
    protected Forms form = Forms.Living;
    public Forms Form { get { return form; } }

    //    mesh = GetComponent<NavMeshAgent>();

    Rigidbody body;

    private void OnEnable()
    {
        curHealth = maxHealth;

        if (enemyInitialized)
        {
            transform.position = SelectSpawnPosition();
        }

        enemyInitialized = true;
    }

    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    //This timer is for the attack cooldown for AI, but at this time it's using the exit time to trigger when to allow it to attack again
    void Update() 
    {
        if (actionCooldown > 0)
        {
            actionCooldown -= Time.deltaTime;
        }

        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
    }

    Vector3 SelectSpawnPosition()
    {
        //Selects a random spawn location for the enemy
        float randomXPos = Random.Range(DungeonGenerator.Instance.GetCurrentRoom().GetRoomObject().westCamBoundary + wallOffsetToSpawn,
            DungeonGenerator.Instance.GetCurrentRoom().GetRoomObject().eastCamBoundary - wallOffsetToSpawn);
        float randomZPos = Random.Range(DungeonGenerator.Instance.GetCurrentRoom().GetRoomObject().southCamBoundary + wallOffsetToSpawn,
            DungeonGenerator.Instance.GetCurrentRoom().GetRoomObject().northCamBoundary - wallOffsetToSpawn);

        Vector3 selectedSpawnPosition = new Vector3(randomXPos, transform.position.y, randomZPos);

        //Checks if the selected spawn location is too close to an obstacle in the room
        foreach (GameObject obstacle in DungeonGenerator.Instance.GetCurrentRoom().GetSpawnedModel().obstacles)
        {
            if (Vector3.Distance(selectedSpawnPosition, obstacle.transform.position) < wallOffsetToSpawn)
            {
                return SelectSpawnPosition();
            }
        }

        return selectedSpawnPosition;
    }

    virtual public void EnemyDeath() 
    {
        //Children are supposed to override so that they can have unique death events
        GetComponentInChildren<Animator>().SetTrigger("Is_Dead");

        EnemyPool.Instance.AddBoarToPool(gameObject);
    }

    /// <summary>
    ///  Take Damage applies damage and, if second argument is true, gives knockback based on the damage (using default direction)
    /// </summary>
    public void TakeDamage(float damageAmount) //Take Damage applies damage and gives knockback based on the damage, unless false
    {
        curHealth -= damageAmount;

        if(curHealth <= 0) 
        {
            EnemyDeath();
        }
    }

    /// <summary>
    ///  2 Overloads: With direction, and with default direction (agent pos - player pos)
    /// </summary>
    //public void ApplyKnockback(Vector3 direction, float knockBack)
    //{
    //    //Vector3 dir = transform.position - PlayerController.Instance.transform.position;
    //    direction.y = 0f;
    //    GetComponent<Rigidbody>().AddForce(direction.normalized * knockBack, ForceMode.VelocityChange);
    //    //float force = weight / damageAmount;
    //    //GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * force);

    //}
    //public void ApplyKnockback(float knockback)
    //{
    //    Vector3 direction = transform.position - PlayerController.Instance.transform.position;
    //    direction.y = 0f;
    //    GetComponent<Rigidbody>().AddForce(direction.normalized * knockback, ForceMode.VelocityChange);
    //    //float force = weight / damageAmount;
    //    //GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * force);

    //}

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
}
