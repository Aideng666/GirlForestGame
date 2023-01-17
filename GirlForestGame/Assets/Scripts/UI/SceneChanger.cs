using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator animator;
    public int sceneToLoad;
    public bool shouldLoadSceneAutomatically = false;
    [SerializeField] GameObject splashUIPanel;
    public bool canPressStartButton = false;

    // Update is called once per frame
    void Update()
    {
        if (canPressStartButton)
        {
            if (InputManager.Instance.Proceed())
            {
                //Debug.Log("CHECK");
                splashUIPanel.SetActive(false);
            }
        }      
    }

    public void FadeToScene()
    {
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        if (shouldLoadSceneAutomatically)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void OnFadeInComplete()
    {
        if (shouldLoadSceneAutomatically)
        {
            FadeToScene();
        }     
    }

    public void CanPressToBegin()
    {

    }
}
