using UnityEngine;
using Cinemachine;

public class MenuFunctionManager : MonoBehaviour
{
    [SerializeField] GameObject splashUIPanel;
    [SerializeField] GameObject mainMenuUIPanel;
    [SerializeField] AnimationEvents animEventsObj;
    [SerializeField] Animator cineCamAnimator;
    [SerializeField] AnimationEvents camEventObj;
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject quitButton;
    [SerializeField] Animator settingsButtonAnimator;

    bool moveCam = false;

    bool hasClickedSettings = false;

    private void Start()
    {
        mainMenuUIPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Checks for the Press A to begin to appear
        if (animEventsObj.GetCanPressStart())
        {
            // If the Press A text shows, user can provide input
            if (InputManager.Instance.Proceed())
            {
                // Clears the UI panel
                splashUIPanel.SetActive(false);
                // Allows camera to move
                moveCam = true;
            }
        }

        if (moveCam)
        {
            cineCamAnimator.SetBool("canProceed", true);
        }

        if (camEventObj.GetOnCamTranComplete())
        {
            mainMenuUIPanel.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Settings()
    {
        hasClickedSettings = !hasClickedSettings;
        
        if (!hasClickedSettings)
        {
            //playButton.SetActive(true);
            //quitButton.SetActive(true);
            settingsButtonAnimator.SetBool("hasClickedSettings", false);
        }
        else
        {
            //playButton.SetActive(false);
            //quitButton.SetActive(false);
            settingsButtonAnimator.SetBool("hasClickedSettings", true);

        }
        //Debug.Log(hasClickedSettings.ToString());
    }
}
