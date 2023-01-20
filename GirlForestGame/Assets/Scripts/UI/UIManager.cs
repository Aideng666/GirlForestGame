using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] RoomTransition fadePanel;
    [SerializeField] GameObject controlsPanel;
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
}
