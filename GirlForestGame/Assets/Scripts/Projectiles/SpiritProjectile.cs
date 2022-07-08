using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpiritProjectile : MonoBehaviour
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
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player) && isActive && player.Form == Forms.Spirit)
        {
            player.ApplyKnockback((player.transform.position - transform.position).normalized, 20);

            Destroy(gameObject);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.TryGetComponent<PlayerController>(out PlayerController player) && isActive && player.Form == Forms.Spirit)
    //    {
    //        player.ApplyKnockback((player.transform.position - transform.position).normalized, 20);

    //        Destroy(gameObject);
    //    }
    //}

    public void SetActive(bool active)
    {
        isActive = active;
    }
}
