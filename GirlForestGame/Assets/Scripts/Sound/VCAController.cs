using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VCAController : MonoBehaviour
{
    private Slider Slider;

    private FMOD.Studio.VCA Vcacontroller;
    public string VCAName;

    [SerializeField] private float vcaVolume;
    // Start is called before the first frame update
    void Start()
    {
        Vcacontroller = FMODUnity.RuntimeManager.GetVCA("vca:/" + VCAName);
        Slider = GetComponent<Slider>();
        Vcacontroller.getVolume(out vcaVolume);
    }

    public void SetVolume(float volume)
    {
        Vcacontroller.setVolume(volume);
        Vcacontroller.getVolume(out vcaVolume);

    }
}
