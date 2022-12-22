using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    public delegate void OnHit(List<EnemyData> enemyHit);
    public static event OnHit OnBowHit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        transform.parent = collision.gameObject.transform;

        if (collision.gameObject.TryGetComponent(out EnemyData enemy) && enemy.form == PlayerController.Instance.Form)
        {
            enemy.ApplyKnockback(transform.forward, 2);
            enemy.TakeDamage(PlayerController.Instance.playerAttributes.BowDamage);

            List<EnemyData> enemiesHit = new List<EnemyData>();

            enemiesHit.Add(enemy);

            OnBowHit?.Invoke(enemiesHit);
        }
    }
}
