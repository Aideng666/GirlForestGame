using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] ShopItemTypes itemType;
    [SerializeField] TextMeshProUGUI priceText;
    ShopItemInfo itemInfo;

    TotemObject chosenTotem;

    // Start is called before the first frame update
    void Start()
    {
        priceText.enabled = false;

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

    public void Buy()
    {
        if (PlayerController.Instance.playerInventory.GetMoneyAmount() >= itemInfo.value)
        {
            float randomXDir = Random.Range(-1f, 1f);
            float randomZDir = Random.Range(-1f, 1f);
            float randomDistance = Random.Range(1f, 3f);

            PlayerController.Instance.playerInventory.ModifyMoney(-itemInfo.value);

            GameObject item = Instantiate(itemInfo.item, new Vector3(transform.position.x, itemInfo.item.transform.position.y, transform.position.z), Quaternion.identity);

            item.transform.DOJump(item.transform.position + new Vector3(randomXDir, 0, randomZDir).normalized * randomDistance, 1, 2, 1f).SetEase(Ease.Linear);

            if (item.TryGetComponent(out TotemPickup totem))
            {
                totem.ChooseTotem(chosenTotem);
            }

            gameObject.SetActive(false);
        }
    }

    public void SetText()
    {
        priceText.enabled = true;

        switch(itemType)
        {
            case ShopItemTypes.Totem:

                priceText.text = $"{chosenTotem.Totem.totemName} - {itemInfo.value}";

                break;

            case ShopItemTypes.Health:

                priceText.text = $"{itemInfo.value}";

                break;
        }
    }

    public void HideText()
    {
        priceText.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.CompareTag("Player") && PlayerController.Instance.playerInventory.GetMoneyAmount() >= itemInfo.value)
        //{
        //    float randomXDir = Random.Range(-1f, 1f);
        //    float randomZDir = Random.Range(-1f, 1f);
        //    float randomDistance = Random.Range(1f, 3f);

        //    PlayerController.Instance.playerInventory.ModifyMoney(-itemInfo.value);

        //    GameObject item = Instantiate(itemInfo.item, new Vector3(transform.position.x, itemInfo.item.transform.position.y, transform.position.z), Quaternion.identity);

        //    item.transform.DOJump(itemInfo.item.transform.position + new Vector3(randomXDir, 0, randomZDir).normalized * randomDistance, 1, 2, 1f).SetEase(Ease.Linear);

        //    gameObject.SetActive(false);
        //}
    }
}

public enum ShopItemTypes
{
    None,
    Totem,
    Health,
    Marking
}

