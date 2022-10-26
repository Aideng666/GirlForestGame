using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Spirit : ScriptableObject
{
    public string name;
    public List<PlayerAttributes> buffedAttributes;
    public Elements usedElement;
}
