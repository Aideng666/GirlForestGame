using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuFunctionManager : MonoBehaviour
{
    [SerializeField] List<GameObject> canvasList = new List<GameObject>();
    [SerializeField] List<GameObject> menuButtons = new List<GameObject>();
    [SerializeField] List<GameObject> settingsButtons = new List<GameObject>();

    GameObject selectedButton;
    int selectedButtonIndex = 0;

    Tween fadeTween;
    public float startFadeTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeCanvas());
    }

    // Update is called once per frame
    void Update()
    {
        //Changes from splash screen to main menu when button is pushed
        if (canvasList[2].activeInHierarchy && InputManager.Instance.Proceed())
        {
            canvasList[2].SetActive(false);
            canvasList[3].SetActive(true);

            selectedButtonIndex = 0;
            selectedButton = menuButtons[selectedButtonIndex];
        }

        //Main Menu Functionality
        if (canvasList[3].activeInHierarchy)
        {
            if (InputManager.Instance.MoveSelection().y > 0)
            {
                selectedButtonIndex--;

                if (selectedButtonIndex < 0)
                {
                    selectedButtonIndex = 0;
                }

                selectedButton = menuButtons[selectedButtonIndex];
            }
            if (InputManager.Instance.MoveSelection().y < 0)
            {
                selectedButtonIndex++;

                if (selectedButtonIndex >= menuButtons.Count)
                {
                    selectedButtonIndex = menuButtons.Count - 1;
                }

                selectedButton = menuButtons[selectedButtonIndex];
            }

            selectedButton.GetComponent<Button>().Select();
        }

        //Settings Functionality
        if (canvasList[4].activeInHierarchy)
        {
            if (InputManager.Instance.Back())
            {
                CloseSettingsMenu();
            }

            if (InputManager.Instance.MoveSelection().y > 0)
            {
                selectedButtonIndex--;

                if (selectedButtonIndex < 0)
                {
                    selectedButtonIndex = 0;
                }

                selectedButton = settingsButtons[selectedButtonIndex];
            }
            if (InputManager.Instance.MoveSelection().y < 0)
            {
                selectedButtonIndex++;

                if (selectedButtonIndex >= settingsButtons.Count)
                {
                    selectedButtonIndex = settingsButtons.Count - 1;
                }

                selectedButton = settingsButtons[selectedButtonIndex];
            }

            selectedButton.GetComponent<Button>().Select();
        }

        //Credits Functionality
        if (canvasList[5].activeInHierarchy)
        {
            if (InputManager.Instance.Back())
            {
                CloseCreditsMenu();
            }
        }
    }

    public void OpenSettingsMenu()
    {
        ResetButtonScale();

        canvasList[3].SetActive(false);
        canvasList[4].SetActive(true);

        selectedButtonIndex = 0;
        selectedButton = settingsButtons[selectedButtonIndex];
    }

    public void CloseSettingsMenu()
    {
        ResetButtonScale();

        canvasList[4].SetActive(false);
        canvasList[3].SetActive(true);

        selectedButtonIndex = 0;
        selectedButton = menuButtons[selectedButtonIndex];
    }

    public void OpenCreditsMenu()
    {
        ResetButtonScale();

        canvasList[3].SetActive(false);
        canvasList[5].SetActive(true);

        selectedButtonIndex = 0;
        selectedButton = menuButtons[selectedButtonIndex];
    }

    public void PlayGame()
    {
        ResetButtonScale();

        SceneManager.LoadScene("Main");
    }


    public void CloseCreditsMenu()
    {
        ResetButtonScale();

        canvasList[5].SetActive(false);
        canvasList[3].SetActive(true);

        selectedButtonIndex = 0;
        selectedButton = menuButtons[selectedButtonIndex];
    }

    void Fade(CanvasGroup canvasGroup, float endValue, float duration, TweenCallback onEnd)
    {
        if (fadeTween != null)
        {
            fadeTween.Kill(false);
        }

        fadeTween = canvasGroup.DOFade(endValue, duration);
        fadeTween.onComplete += onEnd;
    }

    public void FadeIn(CanvasGroup canvasGroup, float duration)
    {
        Fade(canvasGroup, 1f, duration, () =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }

    public void FadeOut(CanvasGroup canvasGroup, float duration)
    {
        Fade(canvasGroup, 0f, duration, () =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        });
    }

    private IEnumerator FadeCanvas()
    {
        // Wait 1 second
        yield return new WaitForSeconds(1f);
        // Fade in the logo screen canvas
        FadeIn(canvasList[0].GetComponentInChildren<CanvasGroup>(), 1f);
        // Wait
        yield return new WaitForSeconds(2.5f);
        // Fade out the logo screen canvas
        FadeOut(canvasList[0].GetComponentInChildren<CanvasGroup>(), 1f);
        // Wait
        yield return new WaitForSeconds(2.5f);
        // Turn off the logo screen canvas, turn on the FMOD canvas
        canvasList[0].SetActive(false);
        canvasList[1].SetActive(true);
        yield return new WaitForSeconds(1f);
        // Fade in FMOD logo canvas
        FadeIn(canvasList[1].GetComponentInChildren<CanvasGroup>(), 1f);
        // Wait
        yield return new WaitForSeconds(2.5f);
        // Fade out FMOD logo canvas
        FadeOut(canvasList[1].GetComponentInChildren<CanvasGroup>(), 1f);
        // Wait 1 second
        yield return new WaitForSeconds(2.5f);
        // Turns off FMOD canvas, turns on MainMenu canvas
        canvasList[1].SetActive(false);
        canvasList[2].SetActive(true);
        // Wait
        yield return new WaitForSeconds(1f);
        // Fade out to MainMenu canvas
        FadeOut(canvasList[2].GetComponentInChildren<CanvasGroup>(), 1f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetButtonScale()
    {
        foreach (GameObject button in menuButtons)
        {
            button.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        foreach (GameObject button in settingsButtons)
        {
            button.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
