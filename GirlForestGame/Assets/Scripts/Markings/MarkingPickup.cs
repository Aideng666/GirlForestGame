using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MarkingPickup : MonoBehaviour
{
    Spirit chosenSpirit;
    MarkingTypes chosenType;

    private void OnEnable()
    {
        transform.localScale = new Vector3(1, 0, 1);

        transform.DOScale(new Vector3(1, 1, 1), 0.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        ChooseMarking();
    }

    void ChooseMarking()
    {
        Spirit[] possibleSpirits = TypeHandler.GetAllInstances<Spirit>("Spirits");

        int randomIndex = Random.Range(0, possibleSpirits.Length);

        chosenSpirit = possibleSpirits[randomIndex];

        chosenType = (MarkingTypes)Random.Range(0, 2);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.playerMarkings.ChooseWeapon(chosenSpirit, chosenType);

            gameObject.SetActive(false);
        }
    }
}
