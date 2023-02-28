using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ShopItem : InteractableObject
{
    [SerializeField] ShopItemTypes itemType;
    ShopItemInfo itemInfo;

    TotemObject chosenTotem;
    int healthOption; // 0 = half heart | 1 = full heart

    protected override void Start()
    {
        base.Start();

        ChooseItem();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChooseItem()
    {
        ShopItemInfo[] possibleItems = TypeHandler.GetAllInstances<ShopItemInfo>("ShopItems");

        if (itemType == ShopItemTypes.None)
        {
            int randomIndex = Random.Range(0, possibleItems.Length);

            itemInfo = possibleItems[randomIndex];
        }
        else if (itemType == ShopItemTypes.Health)
        {
            List<ShopItemInfo> healthItems = new List<ShopItemInfo>();

            foreach (ShopItemInfo item in possibleItems)
            {
                if (item.itemType == itemType)
                {
                    healthItems.Add(item);
                }
            }

            itemInfo = healthItems[Random.Range(0, 2)];
        }
        else
        {
            foreach (ShopItemInfo item in possibleItems)
            {
                if (item.itemType == itemType)
                {
                    itemInfo = item;
                }
            }
        }

        if (itemInfo.item.TryGetComponent(out TotemPickup totem))
        {
            chosenTotem = totem.GetRandomTotem();
        }
    }

    public override void Pickup()
    {
        if (PlayerController.Instance.playerInventory.GetMoneyAmount() >= itemInfo.value)
        {
            float randomXDir = Random.Range(-1f, 1f);
            float randomZDir = Random.Range(-1f, 1f);
            float randomDistance = Random.Range(1f, 3f);

            PlayerController.Instance.playerInventory.ModifyMoney(-itemInfo.value);
            GameObject item = null;

            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main"))
            {
                item = Instantiate(itemInfo.item, new Vector3(transform.position.x, itemInfo.item.transform.position.y, transform.position.z), Quaternion.identity, DungeonGenerator.Instance.GetCurrentRoom().transform);
            }
            else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Tutorial"))
            {
                item = Instantiate(itemInfo.item, new Vector3(transform.position.x, itemInfo.item.transform.position.y, transform.position.z), Quaternion.identity);
            }

            item.transform.DOJump(item.transform.position + new Vector3(randomXDir, 0, randomZDir).normalized * randomDistance, 1, 2, 1f).SetEase(Ease.Linear);

            if (item.TryGetComponent(out TotemPickup totem))
            {
                totem.ChooseTotem(chosenTotem);
            }

            gameObject.SetActive(false);
        }
    }

    public override void SetText()
    {
        popupText.enabled = true;

        switch(itemType)
        {
            case ShopItemTypes.Totem:

                popupText.text = $"{chosenTotem.Totem.totemName} - {itemInfo.value}";

                break;

            case ShopItemTypes.Health:

                popupText.text = $"{itemInfo.value}";

                break;
        }
    }
}

public enum ShopItemTypes
{
    None,
    Totem,
    Health,
    Marking
}

