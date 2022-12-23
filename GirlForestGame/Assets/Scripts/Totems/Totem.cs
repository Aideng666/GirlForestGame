using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Totem : MonoBehaviour
{
    protected TotemTypes totemType;
    protected PlayerController player;
    protected string totemName;

    protected bool totemInInventory = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public virtual void ApplyEffect()
    {

    }

    public TotemTypes GetTotemType()
    {
        return totemType;
    }
}

public enum TotemTypes
{
    Permanent,
    OnTrigger,
    Constant
}

