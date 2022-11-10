using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;
    }

    public void ActivateAttackHitbox(int attackNum)
    {
        //player.SwordStyle.ActivateHitbox(attackNum);
    }
}
