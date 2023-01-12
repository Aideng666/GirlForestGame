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
    public float curHealth = 1f;
    public float maxHealth = 1f;

    //Cooldown between each attack from the condition
    public float actionCooldown = 0f;

    //May not be needed
    public float weight = 1f;

    //What damage an attack does. Down the line this can become a list for the different attacks the AI can have
    public float attackDamage = 1f;

    //reference to navmesh for knockback
  //  NavMeshAgent mesh;

    //CHANGE THIS TO BE MORE FLEXIBLE
    protected Forms form = Forms.Living;
    public Forms Form { get { return form; } }

    //private void Start()
    //{
    //    mesh = GetComponent<NavMeshAgent>();
    //}

    //This timer is for the attack cooldown for AI, but at this time it's using the exit time to trigger when to allow it to attack again
    void Update() 
    {
        if (actionCooldown > 0)
        {
            actionCooldown -= Time.deltaTime;
        }
    }

    virtual public void EnemyDeath() 
    {
        //Children are supposed to override so that they can have unique death events
        GetComponentInChildren<Animator>().SetTrigger("Is_Dead");
    }

    /// <summary>
    ///  Take Damage applies damage and, if second argument is true, gives knockback based on the damage (using default direction)
    /// </summary>
    public void TakeDamage(float damageAmount, bool applyKnockback = false) //Take Damage applies damage and gives knockback based on the damage, unless false
    {
        curHealth -= damageAmount;
        if(curHealth <= 0) 
        {
            EnemyDeath();
        }
        if(applyKnockback)
            ApplyKnockback(damageAmount);
    }

    /// <summary>
    ///  2 Overloads: With direction, and with default direction (agent pos - player pos)
    /// </summary>
    public void ApplyKnockback(Vector3 direction, float knockBack) 
    {
        //Vector3 dir = transform.position - PlayerController.Instance.transform.position;
        GetComponentInChildren<Animator>().SetTrigger("Is_Hit");
        direction.y = 0f;
        GetComponent<Rigidbody>().AddForce(direction.normalized * knockBack, ForceMode.VelocityChange);
        //float force = weight / damageAmount;
        //GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * force);

    }
    public void ApplyKnockback(float knockback)
    {
        Vector3 direction = transform.position - PlayerController.Instance.transform.position;
        direction.y = 0f;
        GetComponent<Rigidbody>().AddForce(direction.normalized * knockback, ForceMode.VelocityChange);
        //float force = weight / damageAmount;
        //GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * force);

    }
}
