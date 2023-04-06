using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] RoomTransition fadePanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject deathPanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] GameObject[] pauseMenuButtons;
    [SerializeField] InputActionAsset inputActions;

    [Header("MarkingPanel")]
    [SerializeField] GameObject markingPickupPanel;
    [SerializeField] GameObject bowPanel;
    [SerializeField] GameObject swordPanel;
    [SerializeField] Image bowImage;
    [SerializeField] Image swordImage;
    [SerializeField] TextMeshProUGUI bowDescription;
    [SerializeField] TextMeshProUGUI swordDescription;
    [SerializeField] TextMeshProUGUI bowName;
    [SerializeField] TextMeshProUGUI swordName;
    FMOD.Studio.EventInstance eventInstance;


    int selectedPauseButtonIndex = 0;

    public bool inventoryOpen { get; private set; }
    public bool isPaused { get; private set; }

    public static UIManager Instance { get; set; }

    FMOD.Studio.Bus sfxBus;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
        inventoryOpen = false;
        isPaused = false;

        sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/");
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.Pause())
        {
            pausePanel.SetActive(!pausePanel.activeInHierarchy);

            if (pausePanel.activeInHierarchy)
            {
                selectedPauseButtonIndex = 0;
                //sfxBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
                isPaused = true;
                eventInstance.setPaused(true);
            }
            else
            {
                eventInstance.setPaused(false);
                isPaused = false;
            }

        }

        if (isPaused)
        {
            if (InputManager.Instance.MoveSelection().y > 0)
            {
                selectedPauseButtonIndex--;

                if (selectedPauseButtonIndex < 0)
                {
                    selectedPauseButtonIndex = 0;
                }
            }
            //DPAD DOWN
            else if (InputManager.Instance.MoveSelection().y < 0)
            {
                selectedPauseButtonIndex++;

                if (selectedPauseButtonIndex >= pauseMenuButtons.Length)
                {
                    selectedPauseButtonIndex = pauseMenuButtons.Length - 1;
                }
            }

            for (int i = 0; i < pauseMenuButtons.Length; i++)
            {
                if (i == selectedPauseButtonIndex)
                {
                    pauseMenuButtons[i].GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
                }
                else
                {
                    pauseMenuButtons[i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                }
            }

            if (InputManager.Instance.Proceed())
            {
                switch (selectedPauseButtonIndex)
                {
                    case 0:

                        pausePanel.SetActive(false);

                        isPaused = false;

                        break;

                    case 1:



                        break;

                    case 2:


                        break;
                }
            }

            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }


        if (inventoryOpen)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
    }

    public RoomTransition GetFadePanel()
    {
        return fadePanel;
    }

    public void ResetAllBindings()
    {
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }

        PlayerPrefs.DeleteKey("rebinds");
    }

    public void ToggleDeathScreen()
    {
        sfxBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        deathPanel.SetActive(true);

        isPaused = true;
    }

    public void ToggleWinScreen()
    {
        sfxBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        winPanel.SetActive(true);

        isPaused = true;
    }

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeInHierarchy);

        if (inventoryPanel.activeInHierarchy) 
        {
            inventoryOpen = true;
        }
        else
        {
            inventoryOpen = false;
        }
    }

    public void ToggleMarkingPickupPanel(bool active, Spirit spirit, MarkingTypes type)
    {
        markingPickupPanel.SetActive(active);
        inventoryOpen = active;

        if (active)
        {
            if (type == MarkingTypes.Attribute)
            {
                swordDescription.text = spirit.spiritAttributeDesc;
                bowDescription.text = spirit.spiritAttributeDesc;
                swordName.text = spirit.spiritAttributeName;
                bowName.text = spirit.spiritAttributeName;
            }
            else if (type == MarkingTypes.Element)
            {
                swordDescription.text = spirit.spiritElementSwordDesc;
                bowDescription.text = spirit.spiritElementBowDesc;
                swordName.text = spirit.spiritElementSwordName;
                bowName.text = spirit.spiritElementBowName;
            }
        }
    }

    public void LoadScene(string name)
    {
        deathPanel.SetActive(false);
        winPanel.SetActive(false);
        isPaused = false;

        LoadingScreen.Instance.LoadScene(name);
    }
}
