using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    bool canPressStartButton = false;
    bool showMainMenuUI = false;
    
    public void CanPressStart()
    {
        canPressStartButton = true;
    }

    public bool GetCanPressStart()
    {
        return canPressStartButton;
    }

    public void OnCameraTransitionComplete()
    {
        showMainMenuUI = true;
    }

    public bool GetOnCamTranComplete()
    {
        return showMainMenuUI; 
    }
}