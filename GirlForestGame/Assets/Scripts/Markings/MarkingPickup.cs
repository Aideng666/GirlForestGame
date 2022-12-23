using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MarkingPickup : MonoBehaviour
{
    [SerializeField] float colliderDelay = 1;

    Spirit chosenSpirit = null;
    MarkingTypes chosenType = MarkingTypes.None;

    float timeElasped;

    // Start is called before the first frame update
    void Start()
    {
        if (chosenSpirit == null || chosenType == MarkingTypes.None)
        {
            ChooseMarking();
        }

        GetComponent<Collider>().enabled = false;

        timeElasped = 0;
    }

    private void OnEnable()
    {
        GetComponent<Collider>().enabled = false;

        timeElasped = 0;
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            timeElasped += Time.deltaTime;
        }

        if (timeElasped >= colliderDelay)
        {
            GetComponent<Collider>().enabled = true;
        }
    }

    //This is called when a new marking pickup is spawned to randomly select a spirit and type
    void ChooseMarking()
    {
        Spirit[] possibleSpirits = TypeHandler.GetAllInstances<Spirit>("Spirits");
        int randomIndex = Random.Range(0, possibleSpirits.Length);

        chosenSpirit = possibleSpirits[randomIndex];
        chosenType = (MarkingTypes)Random.Range(0, 2);
    }

    //This is called when swapping one marking for another, it gives the newly spawned totem the correct spirit and type after swapping
    public void ChooseMarking(Spirit spirit, MarkingTypes type)
    {
        chosenSpirit = spirit;
        chosenType = type;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !PlayerController.Instance.playerInventory.IsChoosingWeapon)
        {
            PlayerController.Instance.playerInventory.StartWeaponSelection(chosenSpirit, chosenType);

            gameObject.SetActive(false);
        }
    }
}
