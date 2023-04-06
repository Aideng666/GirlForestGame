using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuMusic : MonoBehaviour
{
    private static FMOD.Studio.EventInstance Music;

    [SerializeField] string Path;

    [HideInInspector] public FMOD.Studio.EventInstance click;

    void Start()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance(Path);
        Music.start();
        Music.release();

    }
    private void OnEnable()
    {
        click = FMODUnity.RuntimeManager.CreateInstance("event:/UI/Click");

    }
    public void clickSound()
    {
        click.start();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space)){
            clickSound();
        }
    }
    private void OnDestroy()
    {
        Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Music.release();
        click.release();

    }

}
