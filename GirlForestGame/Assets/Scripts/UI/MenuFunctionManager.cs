using UnityEngine;

public class MenuFunctionManager : MonoBehaviour
{
    [SerializeField] GameObject splashUIPanel;
    [SerializeField] AnimationEvents animEventsobj;
    

    // Update is called once per frame
    void Update()
    {
        if (animEventsobj.GetCanPressStart())
        {
            if (InputManager.Instance.Proceed())
            {
                splashUIPanel.SetActive(false);
            }
        }
    }
}
