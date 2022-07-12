using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingProjectile : MonoBehaviour
{
    bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player) && isActive)
        {
            player.ApplyKnockback((player.transform.position - transform.position).normalized, 20);

            print("Hit: " + collision.gameObject.name);

            Destroy(gameObject);
        }
    }
    public void SetActive(bool active)
    {
        isActive = active;
    }
}
