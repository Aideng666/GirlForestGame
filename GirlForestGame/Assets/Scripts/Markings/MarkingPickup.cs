using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MarkingPickup : InteractableObject
{
    [SerializeField] float colliderDelay = 1;

    Spirit chosenSpirit = null;
    MarkingTypes chosenType = MarkingTypes.None;
    int markingLevel;
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

        ChooseLevel();
    }

    //This is called when swapping one marking for another, it gives the newly spawned totem the correct spirit and type after swapping
    public void ChooseMarking(Spirit spirit, MarkingTypes type, int level)
    {
        chosenSpirit = spirit;
        chosenType = type;

        if (level > 0 && level < 4)
        {
            ChooseLevel(level);
        }
    }

    public void ChooseLevel(int level = 0)
    {
        if (level == 0)
        {
            markingLevel = NodeMapManager.Instance.GetCurrentMapCycle();
        }
        else if (level < 4)
        {
            markingLevel = level;
        }

        chosenSpirit.SetLevel(markingLevel);
    }

    public override void Pickup()
    {
        if (!PlayerController.Instance.playerInventory.IsChoosingWeapon)
        {
            PlayerController.Instance.playerInventory.StartWeaponSelection(chosenSpirit, chosenType);

            gameObject.SetActive(false);
        }
    }

    public override void SetText()
    {
        popupText.enabled = true;

        popupText.text = $"{chosenSpirit.spiritName} {chosenType}";
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Player") && !PlayerController.Instance.playerInventory.IsChoosingWeapon)
        //{
        //    PlayerController.Instance.playerInventory.StartWeaponSelection(chosenSpirit, chosenType);

        //    gameObject.SetActive(false);
        //}
    }
}
