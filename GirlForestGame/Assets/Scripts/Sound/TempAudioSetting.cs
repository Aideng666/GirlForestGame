using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAudioSetting : MonoBehaviour
{

    private FMOD.Studio.VCA Vcacontroller;
    public string VCAName;

    [SerializeField] private float vcaVolume;
    // Start is called before the first frame update
    void Start()
    {
        Vcacontroller = FMODUnity.RuntimeManager.GetVCA("vca:/" + VCAName);
        Vcacontroller.getVolume(out vcaVolume);
    }

    public void SetVolume(float volume)
    {
        Vcacontroller.setVolume(volume);
        Vcacontroller.getVolume(out vcaVolume);

    }
}
