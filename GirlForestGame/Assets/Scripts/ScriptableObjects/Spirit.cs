using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class Spirit : ScriptableObject
{
    public string spiritName;
    public Sprite spiritAttributeSprite;
    public Sprite spiritElementSprite;
    public List<Attributes> buffedAttributes;
    public Elements usedElement;

    [HideInInspector] public int markingLevel { get; private set; } = 1;

    public void SetLevel(int level)
    {
        markingLevel = level;
    }
}
