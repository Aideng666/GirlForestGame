using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    float chargedArrowDamage;
    public void SetArrowChargeMultiplier(float percentage)
    {
        chargedArrowDamage = PlayerController.Instance.playerAttributes.BowDamage * percentage;

        print($"Adding {percentage} of the total bow damage to this shot");
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        transform.parent = collision.gameObject.transform;

        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy) && enemy.Form == PlayerController.Instance.playerCombat.Form)
        {
            enemy.ApplyKnockback(transform.forward, 2);
            enemy.TakeDamage(PlayerController.Instance.playerAttributes.BowDamage + chargedArrowDamage);

            List<Enemy> enemiesHit = new List<Enemy>();

            enemiesHit.Add(enemy);

            EventManager.Instance.InvokeOnBowHit(enemiesHit);
        }
    }
}
