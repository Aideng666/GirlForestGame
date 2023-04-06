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
    public string spiritMarkingName; // Kindle/Zephyr
    public string spiritAttributeName;
    public string spiritAttributeDesc;
    public string spiritElementBowName;
    public string spiritElementBowDesc;
    public string spiritElementSwordName;
    public string spiritElementSwordDesc;

    [HideInInspector] public int markingLevel { get; private set; } = 1;

    public void SetLevel(int level)
    {
        markingLevel = level;
    }
}
