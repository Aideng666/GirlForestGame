using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomTransition : MonoBehaviour
{
    [SerializeField] float transitionTime = 1;

    Color panelColor;

    // Start is called before the first frame update
    void Start()
    {
        panelColor = gameObject.GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginRoomTransition()
    {
        StartCoroutine(RoomFade());
    }

    public float GetTransitionTime()
    {
        return transitionTime;
    }

    IEnumerator RoomFade()
    {
        float elaspedTime = 0;

        while(elaspedTime < (transitionTime / 2))
        {
            panelColor.a = Mathf.Lerp(0, 1, elaspedTime / (transitionTime / 2));

            gameObject.GetComponent<Image>().color = panelColor;

            elaspedTime += Time.deltaTime;
            yield return null;
        }

        elaspedTime = 0;

        while (elaspedTime < (transitionTime / 2))
        {
            panelColor.a = Mathf.Lerp(1, 0, elaspedTime / (transitionTime / 2));

            gameObject.GetComponent<Image>().color = panelColor;

            elaspedTime += Time.deltaTime;

            yield return null;
        }

        yield return null;
    }
}
