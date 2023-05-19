using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [HideInInspector] public FMOD.Studio.EventInstance SFX;
    public void setSFX(string name)
    {
        SFX = FMODUnity.RuntimeManager.CreateInstance(name);
    }

    public void PlaySFX()
    {
        SFX.start();
    }
    public void ReleaseSFX()
    {
        SFX.release();
    }
}
