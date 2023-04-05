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
                //Selects closest in range
                foreach (GameObject interactable in interactablesInRange)
                {
                    if (Vector3.Distance(PlayerController.Instance.transform.position, interactable.transform.position) < Vector3.Distance(PlayerController.Instance.transform.position, selectedObject.transform.position))
                    {
                        selectedObject = interactable;
                    }
                }
            }

            //Sets text for the interactables
            selectedObject.GetComponent<InteractableObject>().SetText();

            foreach (GameObject interactable in interactablesInRange)
            {
                if (selectedObject != interactable)
                {
                    interactable.GetComponent<InteractableObject>().HideText();
                }
            }
        }
        else
        {
            selectedObject = null;
        }

        if (selectedObject != null && selectedObject.TryGetComponent(out MarkingPickup markingPickup))
        {
            HUD.Instance.HighlightMarkingIcons(markingPickup);
        }
        else if(HUD.Instance != null)
        {
            HUD.Instance.HighlightMarkingIcons(null);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out InteractableObject item))
        {
            interactablesInRange.Add(item.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out InteractableObject item))
        {
            if (selectedObject == item.gameObject)
            {
                selectedObject = null;
            }

            interactablesInRange.Remove(item.gameObject);

            item.HideText();
        }
    }

    public void ResetSelectedObject()
    {
        selectedObject = null;
    }

    public void ResetInteractablesInRange()
    {
        interactablesInRange = new List<GameObject>();
    }
}
