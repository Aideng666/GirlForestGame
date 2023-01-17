using UnityEngine;

public class MenuFunctionManager : MonoBehaviour
{
    [SerializeField] GameObject splashUIPanel;
    [SerializeField] AnimationEvents animEventsObj;
    [SerializeField] Animator cineCamAnimator;

    bool moveCam = false;

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
    }
}
