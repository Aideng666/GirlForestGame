using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTargetCollider : MonoBehaviour
{
    [SerializeField] Weapons weapon;
    PlayerController player;

    private void Start()
    {
        player = PlayerController.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out EnemyData enemy))
        {
            if (weapon == Weapons.Bow)
            {
                player.playerCombat.AddBowTarget(enemy);
            }
            else if (weapon == Weapons.Sword)
            {
                player.playerCombat.AddSwordTarget(enemy);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out EnemyData enemy))
        {
            if (weapon == Weapons.Bow)
            {
                player.playerCombat.RemoveBowTarget(enemy);
            }
            else if (weapon == Weapons.Sword)
            {
                player.playerCombat.RemoveSwordTarget(enemy);
            }
        }
    }
}

