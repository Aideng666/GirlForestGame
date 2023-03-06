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
    [SerializeField] Animator creditsButtonAnimator;

    bool moveCam = false;

    bool hasClickedSettings = false;
    bool hasClickedCredits = false;

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
            settingsButtonAnimator.SetBool("hasClickedSettings", false);
        }
        else
        {
            settingsButtonAnimator.SetBool("hasClickedSettings", true);

        }
    }

    public void Credits()
    {
        hasClickedCredits = !hasClickedCredits;

        if (!hasClickedCredits)
        {
            creditsButtonAnimator.SetBool("hasClickedCredits", false);
        }
        else
        {
            creditsButtonAnimator.SetBool("hasClickedCredits", true);
        }
    }

    public void Play()
    {
        LoadingScreen.Instance.LoadScene("Main");
    }
}
