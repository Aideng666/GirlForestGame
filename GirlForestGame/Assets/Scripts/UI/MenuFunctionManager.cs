using UnityEngine;

public class MenuFunctionManager : MonoBehaviour
{
    [SerializeField] GameObject splashUIPanel;
    [SerializeField] GameObject mainMenuUIPanel;
    [SerializeField] AnimationEvents animEventsObj;
    [SerializeField] Animator cineCamAnimator;
    [SerializeField] AnimationEvents camEventObj;
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject settingsButton;
    [SerializeField] GameObject tutorialButton;
    [SerializeField] GameObject quitButton;
    [SerializeField] GameObject audioButton;
    [SerializeField] GameObject controlsButton;
    [SerializeField] GameObject creditsButton;
    [SerializeField] GameObject audioPanel;
    [SerializeField] GameObject controlsPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] Animator settingsButtonAnimator;
    [SerializeField] GameObject settingsUIPanel;
    [SerializeField] GameObject settingsInfoPanels;

    bool moveCam = false;

    bool hasClickedSettings = false;

    GameObject[] menuButtons;
    int selectedButtonIndex = 0;

    private void Start()
    {
        mainMenuUIPanel.SetActive(false);
        menuButtons = new GameObject[] { playButton, settingsButton, tutorialButton, quitButton };
        selectedButtonIndex = 0;
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

        //Checks for controller input for menu
        if (mainMenuUIPanel.activeSelf)
        {
            if (settingsUIPanel.activeSelf) 
            {
                menuButtons = new GameObject[4] { settingsButton, audioButton, controlsButton, creditsButton};
            }
            else
            {
                menuButtons = new GameObject[4] { playButton, settingsButton, tutorialButton, quitButton };
            }

            //DPAD UP
            if(InputManager.Instance.MoveSelection().y > 0)
            {
                selectedButtonIndex--;

                if (selectedButtonIndex < 0)
                {
                    selectedButtonIndex = 0;
                }
            }
            //DPAD DOWN
            else if (InputManager.Instance.MoveSelection().y < 0)
            {
                selectedButtonIndex++;

                if (selectedButtonIndex >= menuButtons.Length)
                {
                    selectedButtonIndex = menuButtons.Length - 1;
                }
            }

            for (int i = 0; i < menuButtons.Length; i++)
            {
                if (i == selectedButtonIndex)
                {
                    //menuButtons[selectedButtonIndex].transform.localScale = Vector3.one * 1.9f;
                    menuButtons[i].GetComponent<RectTransform>().localScale = new Vector3(1.3f, 1.3f, 1.3f);
                }
                else
                {
                    //menuButtons[selectedButtonIndex].transform.localScale = Vector3.one * 1.68f;
                    menuButtons[i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
                }
            }

            if (InputManager.Instance.Proceed())
            {
                switch (selectedButtonIndex)
                {
                    case 0:

                        if (settingsUIPanel.activeSelf)
                        {
                            Settings();
                        }
                        else
                        {
                            Play();
                        }

                        break;

                    case 1:

                        if (settingsUIPanel.activeSelf)
                        {
                            AudioPanel();
                        }
                        else
                        {
                            Settings();
                        }

                        break;

                    case 2:

                        if (settingsUIPanel.activeSelf)
                        {
                            ControlsPanel();
                        }
                        else
                        {
                            Tutorial();
                        }

                        break;

                    case 3:

                        if (settingsUIPanel.activeSelf)
                        {
                            CreditsPanel();
                        }
                        else
                        {
                            QuitGame();
                        }

                        break;
                }
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public void AudioPanel()
    {
        audioPanel.SetActive(!audioPanel.activeSelf);
        controlsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void ControlsPanel()
    {
        controlsPanel.SetActive(!controlsPanel.activeSelf);
        audioPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void CreditsPanel()
    {
        creditsPanel.SetActive(!creditsPanel.activeSelf);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }
    

    public void Settings()
    {
        hasClickedSettings = !hasClickedSettings;
        
        if (!hasClickedSettings)
        {
            settingsButtonAnimator.SetBool("hasClickedSettings", false);
            settingsUIPanel.SetActive(false);
            settingsInfoPanels.SetActive(false);
            audioPanel.SetActive(false);
            controlsPanel.SetActive(false);
            creditsPanel.SetActive(false);
            selectedButtonIndex = 1;
        }
        else
        {
            settingsButtonAnimator.SetBool("hasClickedSettings", true);
            settingsUIPanel.SetActive(true);
            settingsInfoPanels.SetActive(true);
            selectedButtonIndex = 0;
        }
    }

    public void Play()
    {
        LoadingScreen.Instance.LoadScene("Main");
    }

    public void Tutorial()
    {
        LoadingScreen.Instance.LoadScene("Tutorial");
    }
}
