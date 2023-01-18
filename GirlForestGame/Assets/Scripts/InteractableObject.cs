using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI popupText;
    public virtual void Pickup()
    {

    }

    public virtual void SetText()
    {
        popupText.enabled = true;
    }

    public void HideText()
    {
        popupText.enabled = false;
    }
}
