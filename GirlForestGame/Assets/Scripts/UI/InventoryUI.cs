using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] List<GameObject> markingIconsList = new List<GameObject>();
    [SerializeField] Image totemDescriptionImage;
    [SerializeField] TextMeshProUGUI totemDescriptionName;
    [SerializeField] TextMeshProUGUI totemDescription;
    [SerializeField] Image markingDescriptionImage;
    [SerializeField] TextMeshProUGUI markingDescriptionName;
    [SerializeField] TextMeshProUGUI markingDescription;
    [SerializeField] Image inventoryCursor;

    //List<GameObject> totemIcons = new List<GameObject>();
    Dictionary<GameObject, TotemObject> totemIcons = new Dictionary<GameObject, TotemObject>();
    //Dictionary<GameObject, Spirit> markingIcons = new Dictionary<GameObject, Spirit>();
    List<GameObject> iconList = new List<GameObject>();

    GameObject selectedIcon = null;

    float cursorMoveSpeed = 750f;

    public static InventoryUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < markingIconsList.Count; i++ ) 
        {
            iconList.Add(markingIconsList[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 cursorMoveInput = InputManager.Instance.Move();

        if (cursorMoveInput != Vector2.zero)
        {
            inventoryCursor.transform.Translate(cursorMoveInput * cursorMoveSpeed * Time.deltaTime);
        }

        //if (PlayerController.Instance.MouseControlActive())
        //{
        //    inventoryCursor.transform.position = Camera.main.ScreenToWorldPoint();
        //}

        bool iconFound = false;
        foreach (GameObject item in iconList)
        {
            if (Overlaps(inventoryCursor.gameObject, item))
            {
                item.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
                selectedIcon = item;
                iconFound = true;
            }
            else
            {
                item.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        if (iconFound)
        {
            if (selectedIcon.GetComponent<Image>().sprite == markingIconsList[0].GetComponent<Image>().sprite)
            {
                markingDescriptionImage.sprite = markingIconsList[0].GetComponent<Image>().sprite;
                markingDescriptionName.text = PlayerController.Instance.playerMarkings.markings[0].spiritName;
                //markingDescription.text = PlayerController.Instance.playerMarkings.markings[0];
            }
            else if (selectedIcon.GetComponent<Image>().sprite == markingIconsList[1].GetComponent<Image>().sprite)
            {
                markingDescriptionImage.sprite = markingIconsList[1].GetComponent<Image>().sprite;
                markingDescriptionName.text = PlayerController.Instance.playerMarkings.markings[1].spiritName;
            }
            else if (selectedIcon.GetComponent<Image>().sprite == markingIconsList[2].GetComponent<Image>().sprite)
            {
                markingDescriptionImage.sprite = markingIconsList[2].GetComponent<Image>().sprite;
                markingDescriptionName.text = PlayerController.Instance.playerMarkings.markings[2].spiritName;
            }
            else if (selectedIcon.GetComponent<Image>().sprite == markingIconsList[3].GetComponent<Image>().sprite)
            {
                markingDescriptionImage.sprite = markingIconsList[3].GetComponent<Image>().sprite;
                markingDescriptionName.text = PlayerController.Instance.playerMarkings.markings[3].spiritName;
            }
            else
            {
                totemDescriptionImage.sprite = selectedIcon.GetComponent<Image>().sprite;
                totemDescriptionName.text = totemIcons[selectedIcon].Totem.totemName;
                totemDescription.text = totemIcons[selectedIcon].Totem.totemDescription;
            }
        }
    }

    public void AddTotemIcon(GameObject icon, TotemObject totem)
    {
        totemIcons.Add(icon, totem);
        iconList.Add(icon);
    }

    public void UpdateMarkingIcon(Spirit spirit, Weapons weapon, MarkingTypes type)
    {
        if (weapon == Weapons.Sword)
        {
            if (type == MarkingTypes.Attribute)
            {
                markingIconsList[0].GetComponent<Image>().sprite = spirit.spiritAttributeSprite;
            }
            else if (type == MarkingTypes.Element)
            {
                markingIconsList[1].GetComponent<Image>().sprite = spirit.spiritElementSprite;
            }
        }
        else if (weapon == Weapons.Bow)
        {
            if (type == MarkingTypes.Attribute)
            {
                markingIconsList[2].GetComponent<Image>().sprite = spirit.spiritAttributeSprite;
            }
            else if (type == MarkingTypes.Element)
            {
                markingIconsList[3].GetComponent<Image>().sprite = spirit.spiritElementSprite;
            }
        }
    }

    bool Overlaps(GameObject obj1, GameObject obj2)
    {
        RectTransform trans1 = obj1.GetComponent<RectTransform>();
        RectTransform trans2 = obj2.GetComponent<RectTransform>();

        // Calculate the AABBs of the two objects
        Rect rect1 = trans1.rect;
        Rect rect2 = trans2.rect;

        Vector2 obj1Min = (Vector2)trans1.position + (Vector2)trans1.TransformVector(rect1.min);
        Vector2 obj1Max = (Vector2)trans1.position + (Vector2)trans1.TransformVector(rect1.max);
        Vector2 obj2Min = (Vector2)trans2.position + (Vector2)trans2.TransformVector(rect2.min);
        Vector2 obj2Max = (Vector2)trans2.position + (Vector2)trans2.TransformVector(rect2.max);

        // Check for overlap in each dimension
        bool xOverlap = (obj1Max.x >= obj2Min.x && obj1Min.x <= obj2Max.x);
        bool yOverlap = (obj1Max.y >= obj2Min.y && obj1Min.y <= obj2Max.y);

        // If there is overlap in both dimensions, the objects are colliding
        return xOverlap && yOverlap;
    }
}
