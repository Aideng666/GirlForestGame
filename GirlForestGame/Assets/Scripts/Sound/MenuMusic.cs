using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuMusic : MonoBehaviour
{
    private static FMOD.Studio.EventInstance Music;

    [SerializeField] string Path;
    void Start()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance(Path);
        Music.start();
        Music.release();

    }

    private void OnDestroy()
    {
        Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

}
