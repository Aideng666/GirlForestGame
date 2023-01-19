using UnityEngine;
using Cinemachine;

public class MenuFunctionManager : MonoBehaviour
{
    [SerializeField] GameObject splashUIPanel;
    //[SerializeField] GameObject mainMenuUIPanel;
    [SerializeField] AnimationEvents animEventsObj;
    [SerializeField] Animator cineCamAnimator;
    [SerializeField] AnimationEvents camEventObj;

    [SerializeField] CinemachineVirtualCamera currentCam;
    [SerializeField] CinemachineVirtualCamera firstCamTarget;

    bool moveCam = false;

    private void Start()
    {
        //mainMenuUIPanel.SetActive(false);
        currentCam.Priority++;
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
            UpdateCamera(firstCamTarget);
            //cineCamAnimator.SetBool("canProceed", true);
        }

        if (camEventObj.GetOnCamTranComplete())
        {
            //mainMenuUIPanel.SetActive(true);
        }

        //if (cineCamAnimator.GetBool("canProceed"))
        //{
        //    UpdateCamera(firstCamTarget);
        //}
    }

    public void UpdateCamera(CinemachineVirtualCamera target)
    {
        currentCam.Priority--;

        currentCam = target;

        currentCam.Priority++;
    }
}
