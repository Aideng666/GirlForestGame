using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [Header("Attack Indicators")]
    [SerializeField] Image attackIndicatorImage;
    [SerializeField] Sprite astralIndicatorSprite;
    [SerializeField] Sprite terrestrialIndicatorSprite;

    Camera mainCam;
    float attackIndicationDuration = 1;

    // Start is called before the first frame update
    void Start()
    {
        attackIndicatorImage.enabled = false;
        mainCam = Camera.main;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(30, 45, 0);
    }

    public void IndicateAttack(Planes plane, float duration)
    {
        attackIndicatorImage.enabled = true;
        attackIndicationDuration = duration;

        if (plane == Planes.Terrestrial)
        {
            attackIndicatorImage.sprite = terrestrialIndicatorSprite;
        }
        else if (plane == Planes.Astral)
        {
            attackIndicatorImage.sprite = astralIndicatorSprite;
        }

        StartCoroutine(DeactivateAttackIndicator());
    }

    IEnumerator DeactivateAttackIndicator()
    {
        yield return new WaitForSeconds(attackIndicationDuration);

        attackIndicatorImage.enabled = false;
    }
}
