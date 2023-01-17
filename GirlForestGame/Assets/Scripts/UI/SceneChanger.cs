using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator animator;
    public int sceneToLoad;
    public bool shouldLoadSceneAutomatically = false;

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
}
