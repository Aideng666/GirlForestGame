using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marking : MonoBehaviour
{
    protected int level;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}

public enum Spirits
{
    Hawk,
    Fox
}

public enum MarkingTypes
{
    Attribute,
    Element,
    None
}

public enum Weapons
{
    Sword,
    Bow,
    None
}

public enum Elements
{
    Fire,
    Wind,
    None
}

