using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuFunctionManager : MonoBehaviour
{
    [SerializeField] List<GameObject> canvasList = new List<GameObject>();
    [SerializeField] GameObject pressPlayPrompt;

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
}
