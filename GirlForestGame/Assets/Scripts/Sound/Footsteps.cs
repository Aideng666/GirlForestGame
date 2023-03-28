using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{   
    FMOD.Studio.EventInstance FootstepsEvent;

    public PlayerController pControl;
    bool playerismoving;
    public float paceSpeed;
    //private float WoodValue;
    //private float GrassValue;

    void Start()
    {
        FootstepsEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Footsteps");
        //FootstepsEvent.getParameterByName("Wood", out WoodValue);
        //FootstepsEvent.getParameterByName("Metal", out MetalValue);
        //FootstepsEvent.getParameterByName("Grass", out GrassValue);

        if (FootstepsEvent.isValid())
        {
            FootstepsEvent.setVolume(1.0f);
        }
        //InvokeRepeating("CallFootsteps", 1, paceSpeed);
    }

    void Update()
    {
        //Debug.Log("Wood: " + WoodValue);
        //Debug.Log("Grass: " + GrassValue);
       // FootstepsEvent.setParameterByName("Wood", WoodValue);
        //FootstepsEvent.setParameterByName("Metal", MetalValue);
        //FootstepsEvent.setParameterByName("Grass", GrassValue);

        if (InputManager.Instance.Move().magnitude > 0.1f)//(Input.GetAxis("Vertical") >= 0.01f || Input.GetAxis("Horizontal") >= 0.01f || Input.GetAxis("Vertical") <= -0.01f || Input.GetAxis("Horizontal") <= -0.01f)
        {
            playerismoving = true; 
            FootstepsEvent.keyOff();
            Debug.Log("p " + playerismoving);

        }
        else
        {
            playerismoving = false;
            FootstepsEvent.start();
            //FootstepsEvent.keyOff();
            //FootstepsEvent.release();
            Debug.Log("p " + playerismoving);
        }
    }

    void CallFootsteps()
    {
        if (playerismoving == true)
        {
            //FMODUnity.RuntimeManager.PlayOneShot(InputFootsteps);

            //FootstepsEvent.start();
           // FootstepsEvent.release();
        }
        else if (playerismoving == false)
        {
            //FootstepsEvent.keyOff();
          // FootstepsEvent.release();
            
        }
    }

    void OnDisable()
    {
        playerismoving = false;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //Debug.Log("hit");
    //    //float FadeSpeed = 10f;
    //    playerisgrounded = true;

    //    if (collision.collider.tag == "Wood")
    //    {
    //        WoodValue = 1f;
    //        GrassValue = 0f;
    //    }
    //    //if (collision.collider.tag == "Metal")
    //    //{
    //    //    WoodValue = 0f;
    //    //    MetalValue = 1f;
    //    //    GrassValue = 0f;
    //    //}
    //    if (collision.collider.tag == "Grass")
    //    {
    //        WoodValue = 0f;
    //        GrassValue = 1f;
    //    }
    //}
    void StopAllPlayerEvents()
    {
        FMOD.Studio.Bus playerBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX/Player");
        playerBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

}