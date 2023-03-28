using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialIcons : MonoBehaviour
{
    [SerializeField] Sprite leftStickIcon;
    [SerializeField] Sprite yButtonIcon;
    [SerializeField] Sprite leftBumperIcon;
    [SerializeField] Sprite rightBumperIcon;
    [SerializeField] Sprite aButtonIcon;

    Image iconImage;

    public static TutorialIcons Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        iconImage = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(30, 45, 0);
    }

    public void EnableIcon()
    {
        iconImage.enabled = true;
    }

    public void DisableIcon()
    {
        iconImage.enabled = false;
    }

    public void SwapIcons(ButtonIcons icon)
    {
        switch (icon)
        {
            case ButtonIcons.A:

                iconImage.sprite = aButtonIcon;

                break;

            case ButtonIcons.Y:

                iconImage.sprite = yButtonIcon;

                break;

            case ButtonIcons.LB:

                iconImage.sprite = leftBumperIcon;

                break;

            case ButtonIcons.RB:

                iconImage.sprite = rightBumperIcon;

                break;

            case ButtonIcons.LStick:

                iconImage.sprite = leftStickIcon;

                break;
        }
    }
}

public enum ButtonIcons
{
    A,
    Y,
    LB,
    RB,
    LStick
}
