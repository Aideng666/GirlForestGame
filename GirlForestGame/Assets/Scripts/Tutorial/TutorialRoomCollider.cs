using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialRoomCollider : MonoBehaviour
{
    TutorialRoom room;

    private void Start()
    {
        room = transform.parent.GetComponentInParent<TutorialRoom>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TutorialManager.Instance.NextRoom();
        }
    }
}
