//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TotemPool : MonoBehaviour
//{
//    List<GameObject> availableTotems = new List<GameObject>();

//    public static TotemPool Instance { get; private set; }

//    private void Awake()
//    {
//        Instance = this;
//        CreatePool();
//    }

//    public GameObject GetTotemFromPool(int itemIndex)
//    {
//        var instance = availableTotems[itemIndex];

//        instance.SetActive(true);
//        return instance;
//    }

//    private void CreatePool()
//    {
//        var permanentTotems = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(PermanentTotem));

//        for (int i = 0; i < permanentTotems.Length; i++)
//        {
//            GameObject totemToAdd = new GameObject("Permanent totem", new System.Type[] { permanentTotems[i] });
//            totemToAdd.transform.SetParent(transform);

//            AddTotemToPool(totemToAdd);
//        }
//    }

//    public void AddTotemToPool(GameObject instance)
//    {
//        instance.SetActive(false);
//        availableTotems.Add(instance);
//    }
//}

