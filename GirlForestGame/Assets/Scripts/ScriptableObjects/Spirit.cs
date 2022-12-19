using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Spirit : ScriptableObject
{
    public string spiritName;
    public List<Attributes> buffedAttributes;
    public Elements usedElement;
}
