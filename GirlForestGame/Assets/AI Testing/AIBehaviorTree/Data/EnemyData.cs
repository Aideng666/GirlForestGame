using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //CHANGE THIS TO BE MORE FLEXIBLE
    public Forms form = Forms.Living;

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


    public void TakeDamage(float damageAmount, bool applyKnockback = false) //Take Damage applies damage and gives knockback, unless false
    {
        curHealth -= damageAmount;
        if(curHealth <= 0) 
        {
            EnemyDeath();
        }
        if(applyKnockback)
            ApplyKnockback(Vector3.zero, damageAmount);
    }

    /// <summary>
    ///  !!! DIRECTION DOES NOT WORK (NOT PROGRAMMED) !!!
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="damageAmount"></param>
    public void ApplyKnockback(Vector3 direction, float damageAmount) 
    {
        Vector3 dir = transform.position - PlayerController.Instance.transform.position;
        direction.y = 0f;
        float force = 10;
        GetComponent<Rigidbody>().AddForce(direction.normalized * force, ForceMode.VelocityChange);
        //float force = weight / damageAmount;
        //GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * force);

    }
}
