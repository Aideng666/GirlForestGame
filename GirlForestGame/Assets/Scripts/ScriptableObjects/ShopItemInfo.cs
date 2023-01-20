using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShopItemInfo : ScriptableObject
{
    public ShopItemTypes itemType;
    public int value;
    public GameObject item;
    //public GameObject visualComponent;
}
