using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    bool canPressStartButton = false;

    public void CanPressStart()
    {
        canPressStartButton = true;
    }

    public bool GetCanPressStart()
    {
        return canPressStartButton;
    }
}
