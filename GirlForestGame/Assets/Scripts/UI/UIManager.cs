using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] RoomTransition fadePanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject deathPanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject inventoryPanel;
    [SerializeField] InputActionAsset inputActions;

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

        sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.Pause())
        {
            pausePanel.SetActive(!pausePanel.activeInHierarchy);

            if (pausePanel.activeInHierarchy)
            {
                //sfxBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
                isPaused = true;
            }
            else
            {
                isPaused = false;
            }
        }

        if (isPaused)
        {
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

    public void LoadScene(string name)
    {
        deathPanel.SetActive(false);
        winPanel.SetActive(false);
        isPaused = false;

        LoadingScreen.Instance.LoadScene(name);
    }
}
