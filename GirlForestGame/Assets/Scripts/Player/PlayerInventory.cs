using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] GameObject markingPrefab;
    [SerializeField] int startingMoney;

    //0 = Sword Attribute
    //1 = Sword Element
    //2 = Bow Attribute
    //3 = Bow Element
    Spirit[] markings;

    List<TotemObject> totems;
    public Dictionary<System.Type, int> totemDictionary { get; private set; } // the int value shows how many of each totem the player has

    PlayerController player;

    int moneyAmount;
    bool inventoryOpen = false;

    public bool IsChoosingWeapon { get; private set; }

    void Start()
    {
        markings = new Spirit[] { null, null, null, null };
        totems = new List<TotemObject>();
        totemDictionary = new Dictionary<System.Type, int>();
        moneyAmount =  startingMoney;

        player = GetComponent<PlayerController>();

        foreach (var totemType in System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(Totem)))
        {
            totemDictionary.Add(totemType, 0);
        }
    }

    private void Update()
    {
        foreach (TotemObject totem in totems)
        {
            if (totem.Totem.GetTotemType() == TotemTypes.Constant)
            {
                totem.Totem.ApplyEffect();
            }
        }
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
                    markingPickup.GetComponent<MarkingPickup>().ChooseMarking(spirit, type, spirit.markingLevel);
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
                    markingPickup.GetComponent<MarkingPickup>().ChooseMarking(spirit, type, spirit.markingLevel);
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
                    markingPickup.GetComponent<MarkingPickup>().ChooseMarking(spirit, type, spirit.markingLevel);
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
                    markingPickup.GetComponent<MarkingPickup>().ChooseMarking(spirit, type, spirit.markingLevel);
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
                print($"Putting the level {spirit.markingLevel} {spirit.spiritName} {type.ToString()} on your Sword");

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

    public void AddTotemToList(TotemObject totem)
    {
        //totems.Add(totem);
        print($"{totem.Totem.totemName}");

        System.Type totemType = totem.Totem.GetType();

        if (totemDictionary.ContainsKey(totemType))
        {
            totemDictionary[totemType] += 1;

            if (totemDictionary[totemType] == 1)
            {
                totems.Add(totem);
            }
        }

        if (totem.Totem.GetTotemType() == TotemTypes.OnPickup)
        {
            //totem.Totem.ApplyEffect();

            foreach (TotemObject t in totems)
            {
                if (t.Totem.totemName == totem.Totem.totemName)
                {
                    t.Totem.ApplyEffect();
                }
            }
        }
    }

    public void RemoveTotem(System.Type totemType)
    {
        if (totemDictionary.ContainsKey(totemType))
        {
            totemDictionary[totemType] = 0;

            foreach (TotemObject t in totems)
            {
                if (t.Totem.GetType() == totemType)
                {
                    totems.Remove(t);

                    return;
                }
            }
        }
    }

    public List<TotemObject> GetTotemList()
    {
        return totems;
    }

    public TotemObject GetTotemFromList(System.Type totemType)
    {
        if (totemDictionary.ContainsKey(totemType))
        {
            foreach (TotemObject t in totems)
            {
                if (t.Totem.GetType() == totemType)
                {
                    return t;
                }
            }
        }

        return null;
    }

    public void ModifyMoney(int value)
    {
        moneyAmount += value;

        if (moneyAmount < 0)
        {
            moneyAmount = 0;
        }
    }

    public int GetMoneyAmount()
    {
        return moneyAmount;
    }



    public void ToggleInventory()
    {
        inventoryOpen = !inventoryOpen;

        //if (inventoryOpen)
        //{
        //print($"Sword Attribute: {markings[0]?.spiritName}");
        //print($"Sword Element: {markings[1]?.spiritName}");
        //print($"Bow Attribute: {markings[2]?.spiritName}");
        //print($"Bow Element: {markings[3]?.spiritName}");
        //print("Totems:");

        foreach (TotemObject totem in totems)
        {
            print($"{totem.Totem.totemName}");
        }
        //}
    }

    public Spirit[] GetMarkings()
    {
        return markings;
    }
}
