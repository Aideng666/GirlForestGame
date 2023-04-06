using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MarkingPickup : InteractableObject
{
    [SerializeField] float colliderDelay = 1;
    [SerializeField] GameObject foxMarkingPrefab;
    [SerializeField] GameObject hawkMarkingPrefab;
    [SerializeField] bool isMarkingPair; // If it is a pair, only one out of the two markings can be picked up in a room, if not, they will not dissapear after taking one marking in a room

    public Spirit chosenSpirit { get; private set; } = null;
    public MarkingTypes chosenType { get; private set; } = MarkingTypes.None;
    int markingLevel;
    float timeElasped;

    GameObject spawnedModel = null;

    protected override void Start()
    {
        base.Start();

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

        if (spawnedModel != null)
        {
            spawnedModel.transform.Rotate(Vector3.up, 0.5f);
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

        if (chosenSpirit.spiritName == "Fox")
        {
            spawnedModel = Instantiate(foxMarkingPrefab, transform.position + (Vector3.down * 0.5f), Quaternion.identity, transform);

            Sequence sequence = DOTween.Sequence();

            sequence.SetLoops(99999999);

            sequence.Append(spawnedModel.transform.DOMoveY(2, 2).SetEase(Ease.InOutSine)).Append(spawnedModel.transform.DOMoveY(0.5f, 2).SetEase(Ease.InOutSine));
        }
        else if (chosenSpirit.spiritName == "Hawk")
        {
            spawnedModel = Instantiate(hawkMarkingPrefab, transform.position + (Vector3.down * 0.5f), Quaternion.identity, transform);

            Sequence sequence = DOTween.Sequence();

            sequence.SetLoops(99999999);

            sequence.Append(spawnedModel.transform.DOMoveY(2, 2).SetEase(Ease.InOutSine)).Append(spawnedModel.transform.DOMoveY(0.5f, 2).SetEase(Ease.InOutSine));
        }
    }

    public override void Pickup()
    {
        if (!PlayerController.Instance.playerInventory.IsChoosingWeapon)
        {
            UIManager.Instance.ToggleMarkingPickupPanel(true, chosenSpirit, chosenType);
            PlayerController.Instance.playerInventory.StartWeaponSelection(chosenSpirit, chosenType);

            if (isMarkingPair)
            {
                foreach (MarkingPickup marking in transform.parent.GetComponentsInChildren<MarkingPickup>())
                {
                    marking.gameObject.SetActive(false);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }

            PlayerController.Instance.GetComponentInChildren<InteractRing>().ResetInteractablesInRange();
            //HUD.Instance.HighlightMarkingIcons(null);
        }
    }

    public override void SetText()
    {
        popupText.enabled = true;

        popupText.text = $"{chosenSpirit.spiritName} {chosenType} - Lvl {markingLevel}";
    }
}
