using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [HideInInspector] public FMOD.Studio.EventInstance FootstepsEvent;

    public PlayerController pControl;
    //private float WoodValue;
    //private float GrassValue;

    void OnEnable()
    {
        FootstepsEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Player/Footsteps");
        //FootstepsEvent.getParameterByName("Wood", out WoodValue);
        //FootstepsEvent.getParameterByName("Metal", out MetalValue);
        //FootstepsEvent.getParameterByName("Grass", out GrassValue);
        //InvokeRepeating("CallFootsteps", 1, paceSpeed);
    }

    void Update()
    {
        //Debug.Log("Wood: " + WoodValue);
        //Debug.Log("Grass: " + GrassValue);
       // FootstepsEvent.setParameterByName("Wood", WoodValue);
        //FootstepsEvent.setParameterByName("Metal", MetalValue);
        //FootstepsEvent.setParameterByName("Grass", GrassValue);

        if (InputManager.Instance.Move().magnitude > 0.1f) //(Input.GetAxis("Vertical") >= 0.01f || Input.GetAxis("Horizontal") >= 0.01f || Input.GetAxis("Vertical") <= -0.01f || Input.GetAxis("Horizontal") <= -0.01f)
        {
            FootstepsEvent.start();

        }
        else
        {
            //FootstepsEvent.keyOff();
            FootstepsEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        }
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

}