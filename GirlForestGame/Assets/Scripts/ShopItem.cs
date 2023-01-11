using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI priceText;
    ShopItemInfo itemInfo;

    // Start is called before the first frame update
    void Start()
    {
        ChooseItem();

        //priceText.transform.parent.rotation = Quaternion.Euler(30, 45, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChooseItem()
    {
        ShopItemInfo[] possibleItems = TypeHandler.GetAllInstances<ShopItemInfo>("ShopItems");
        int randomIndex = Random.Range(0, possibleItems.Length);

        itemInfo = possibleItems[randomIndex];

        priceText.text = $"{itemInfo.value}";
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && PlayerController.Instance.playerInventory.GetMoneyAmount() >= itemInfo.value)
        {
            float randomXDir = Random.Range(-1f, 1f);
            float randomZDir = Random.Range(-1f, 1f);
            float randomDistance = Random.Range(1f, 3f);

            PlayerController.Instance.playerInventory.ModifyMoney(-itemInfo.value);

            GameObject item = Instantiate(itemInfo.item, new Vector3(transform.position.x, itemInfo.item.transform.position.y, transform.position.z), Quaternion.identity);

            item.transform.DOJump(itemInfo.item.transform.position + new Vector3(randomXDir, 0, randomZDir).normalized * randomDistance, 1, 2, 1f).SetEase(Ease.Linear);

            gameObject.SetActive(false);
        }
    }
}
