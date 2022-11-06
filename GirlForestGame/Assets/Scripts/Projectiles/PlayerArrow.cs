using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
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

        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy enemy) && enemy.Form == PlayerController.Instance.Form)
        {
            enemy.ApplyKnockback(transform.forward, 2);
            enemy.TakeDamage(PlayerController.Instance.BowDamage);
        }
    }
}
