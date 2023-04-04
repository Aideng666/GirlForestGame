using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] RoomTransition fadePanel;
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject deathPanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] InputActionAsset inputActions;

    public static UIManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        controlsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.Instance.Pause())
        {
            controlsPanel.SetActive(!controlsPanel.activeInHierarchy);
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
        deathPanel.SetActive(true);
    }

    public void ToggleWinScreen()
    {
        winPanel.SetActive(true);
    }

    public void LoadScene(string name)
    {
        deathPanel.SetActive(false);
        winPanel.SetActive(false);

        LoadingScreen.Instance.LoadScene(name);
    }
}
