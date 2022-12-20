using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] GameObject markingPrefab;
    [SerializeField] Path markingBouncePath;

    //0 = Sword Attribute
    //1 = Sword Element
    //2 = Bow Attribute
    //3 = Bow Element
    Spirit[] markings;

    List<Totem> totems;

    PlayerController player;

    bool inventoryOpen = false;

    public bool IsChoosingWeapon { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        markings = new Spirit[] { null, null, null, null };
        totems = new List<Totem>();

        player = GetComponent<PlayerController>();
    }

    public void EquipMarking(Spirit spirit, MarkingTypes type, Weapons weapon)
    {
        float randomXDir = Random.Range(-1f, 1f);
        float randomZDir = Random.Range(-1f, 1f);
        float randomDistance = Random.Range(2f, 5f);

        if (weapon == Weapons.Sword)
        {
            if (type == MarkingTypes.Attribute)
            {
                if (markings[0] != null)
                {
                    player.playerMarkings.RemoveMarking(markings[0], type, weapon);

                    GameObject markingPickup = Instantiate(markingPrefab, new Vector3(transform.position.x, markingPrefab.transform.position.y, transform.position.z) , Quaternion.identity);

                    markingPickup.transform.DOJump(markingPickup.transform.position + new Vector3(randomXDir, 0, randomZDir).normalized * randomDistance, 1, 2, 1f).SetEase(Ease.Linear);
                    markingPickup.GetComponent<MarkingPickup>().ChooseMarking(spirit, type);
                }

                markings[0] = spirit;
            }
            else if (type == MarkingTypes.Element)
            {
                if (markings[1] != null)
                {
                    player.playerMarkings.RemoveMarking(markings[1], type, weapon);

                    GameObject markingPickup = Instantiate(markingPrefab, new Vector3(transform.position.x, markingPrefab.transform.position.y, transform.position.z), Quaternion.identity);

                    markingPickup.transform.DOJump(markingPickup.transform.position + new Vector3(randomXDir, 0, randomZDir).normalized * randomDistance, 1, 2, 1f).SetEase(Ease.Linear);
                    markingPickup.GetComponent<MarkingPickup>().ChooseMarking(spirit, type);
                }

                markings[1] = spirit;
            }
        }
        else if (weapon == Weapons.Bow)
        {
            if (type == MarkingTypes.Attribute)
            {
                if (markings[2] != null)
                {
                    player.playerMarkings.RemoveMarking(markings[2], type, weapon);

                    GameObject markingPickup = Instantiate(markingPrefab, new Vector3(transform.position.x, markingPrefab.transform.position.y, transform.position.z), Quaternion.identity);

                    markingPickup.transform.DOJump(markingPickup.transform.position + new Vector3(randomXDir, 0, randomZDir).normalized * randomDistance, 1, 2, 1f).SetEase(Ease.Linear);
                    markingPickup.GetComponent<MarkingPickup>().ChooseMarking(spirit, type);
                }

                markings[2] = spirit;
            }
            else if (type == MarkingTypes.Element)
            {
                if (markings[3] != null)
                {
                    player.playerMarkings.RemoveMarking(markings[3], type, weapon);

                    GameObject markingPickup = Instantiate(markingPrefab, new Vector3(transform.position.x, markingPrefab.transform.position.y, transform.position.z), Quaternion.identity);

                    markingPickup.transform.DOJump(markingPickup.transform.position + new Vector3(randomXDir, 0, randomZDir).normalized * randomDistance, 1, 2, 1f).SetEase(Ease.Linear);
                    markingPickup.GetComponent<MarkingPickup>().ChooseMarking(spirit, type);
                }

                markings[3] = spirit;
            }
        }

        player.playerMarkings.UpdateMarking(spirit, type, weapon);
    }

    //Called after picking up a marking before deciding which weapon to put it on
    public void StartWeaponSelection(Spirit spirit, MarkingTypes type)
    {
        StartCoroutine(SelectWeapon(spirit, type));
    }

    IEnumerator SelectWeapon(Spirit spirit, MarkingTypes type)
    {
        IsChoosingWeapon = true;

        bool weaponSelected = false;
        print("Select Sword: Left D-Pad or 1");
        print("Select Bow: Right D-Pad or 2");

        while (!weaponSelected)
        {
            if (InputManager.Instance.SelectSword())
            {
                print($"Putting the {spirit.spiritName} {type.ToString()} on your Sword");

                EquipMarking(spirit, type, Weapons.Sword);

                weaponSelected = true;
            }
            if (InputManager.Instance.SelectBow())
            {
                print($"Putting the {spirit.spiritName} {type.ToString()} on your Bow");

                EquipMarking(spirit, type, Weapons.Bow);

                weaponSelected = true;
            }

            yield return null;
        }

        IsChoosingWeapon = false;
    }

    public void AddTotemToList(Totem totem)
    {
        totems.Add(totem);
    }

    public void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;

        //if (inventoryOpen)
        //{
            print($"Sword Attribute: {markings[0]?.spiritName}");
            print($"Sword Element: {markings[1]?.spiritName}");
            print($"Bow Attribute: {markings[2]?.spiritName}");
            print($"Bow Element: {markings[3]?.spiritName}");
            print("Totems:");

            foreach (Totem totem in totems)
            {
                print($"{totem.name}");
            }
        //}
    }
}
