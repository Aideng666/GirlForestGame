using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator animator;
    public int sceneToLoad;
    public bool shouldLoadSceneAutomatically = false;
    //public GameObject splashUIPanel;
    //bool canPressStartButton = false;

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    FadeToScene();
        //}      
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
}
