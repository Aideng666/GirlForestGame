using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BowAimUI : MonoBehaviour
{
    //[SerializeField] Image background;
    [SerializeField] float maxLengthOfArrow = 10f;

    ArrowGenerator generator;
    MeshRenderer meshRend;
    Color color;

    bool delayDone;
    float defaultLength = 0;
    float timer = 0;

    private void Awake()
    {
        meshRend = GetComponent<MeshRenderer>();
        generator = GetComponent<ArrowGenerator>();
        defaultLength = generator.stemLength;
    }


    // Start is called before the first frame update
    void OnEnable()
    {
        color = meshRend.material.color;
        generator.stemLength = defaultLength;
        delayDone = true;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (delayDone && background.color.a == 25)
        //{
        //    print("Darkening");

        //    background.DOFade(1, 0.5f);

        //    delayDone = false;

        //    StartCoroutine(BackgroundFade(3f));
        //}
        //else if (delayDone && background.color.a == 255)
        //{
        //    print("Lowering");

        //    background.DOFade(0.1f, 1f);

        //    delayDone = false;

        //    StartCoroutine(BackgroundFade(3f));
        //}
        if (timer <= PlayerController.Instance.playerAttributes.BowChargeTime) 
        {
            generator.stemLength = Mathf.Clamp(generator.stemLength + (Time.deltaTime * (maxLengthOfArrow - PlayerController.Instance.playerAttributes.BowChargeTime)), defaultLength, maxLengthOfArrow);
            Debug.Log(timer);
        }
        timer += Time.deltaTime;
        if (delayDone)
        {
            StartCoroutine(BackgroundFade(1.5f));
        }
    }

    //IEnumerator FadeDelay(float duration)
    //{
    //    yield return new WaitForSeconds(duration);

    //    delayDone = true;
    //}

    IEnumerator BackgroundFade(float duration)
    {
        delayDone = false;

        float elaspedTime = 0;

        while (elaspedTime < (duration / 3))
        {
            color.a = Mathf.Lerp(0.1f, 1, elaspedTime / (duration / 3));

            meshRend.material.color = color;



            elaspedTime += Time.deltaTime;
            yield return null;
        }

        elaspedTime = 0;

        while (elaspedTime < (duration / 1.5f))
        {
            color.a = Mathf.Lerp(1, 0, elaspedTime / (duration / 1.5f));

            meshRend.material.color = color;

            elaspedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;

        delayDone = true;
    }
}
