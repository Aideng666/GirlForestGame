using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowSound : MonoBehaviour
{
    public PlayerController pControl;

    private FMOD.Studio.EventInstance BowSFX;
    private FMOD.Studio.EventInstance Special;
    FMOD.Studio.PLAYBACK_STATE PbState;
    // Start is called before the first frame update
    private void Awake()
    {
        BowSFX = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Bow");
        //Special = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Bow");// Special Charge
    }
    private void OnDestroy()
    {
        BowSFX.release();
        //Special.release();
    }

    public void DrawBow()
    {
        BowSFX.start();
    }

    public void FireArrow()
    {
        if (InputManager.Instance.ReleaseArrow())
        {
            Debug.Log("ArrowFire");
            BowSFX.keyOff();//triggerCue

        }
    }

}
