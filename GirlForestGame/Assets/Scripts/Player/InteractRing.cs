using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractRing : MonoBehaviour
{
    List<GameObject> interactablesInRange = new List<GameObject>();
    public GameObject selectedObject { get; private set; } = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (interactablesInRange.Count > 0)
        {
            selectedObject = interactablesInRange[0];

            if (interactablesInRange.Count > 1)
            {
                foreach (GameObject interactable in interactablesInRange)
                {
                    if (Vector3.Distance(PlayerController.Instance.transform.position, interactable.transform.position) < Vector3.Distance(PlayerController.Instance.transform.position, selectedObject.transform.position))
                    {
                        selectedObject = interactable;
                    }
                }
            }
            selectedObject.GetComponent<ShopItem>().SetText();

            foreach (GameObject interactable in interactablesInRange)
            {
                if (selectedObject != interactable)
                {
                    interactable.GetComponent<ShopItem>().HideText();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ShopItem shopItem))
        {
            interactablesInRange.Add(shopItem.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ShopItem shopItem))
        {
            if (selectedObject == shopItem.gameObject)
            {
                selectedObject = null;
            }

            interactablesInRange.Remove(shopItem.gameObject);

            shopItem.HideText();
        }
    }
}
