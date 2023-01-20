using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FontStuff : MonoBehaviour
{
    TMP_Text text;
    string writer;

    [SerializeField] float delayBeforeStart = 0f;
    [SerializeField] float timeBtwnChars = 0.1f;
    [SerializeField] string leadingChar = "";
    [SerializeField] bool leadingCharBeforeDelay = false;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>()!;

        if (text != null)
        {
            writer = text.text;
            text.text = "";

            StartCoroutine("TypeWriter");
        }
    }

    IEnumerator TypeWriter()
    {
        text.text = leadingCharBeforeDelay ? leadingChar : "";

        yield return new WaitForSeconds(delayBeforeStart);

        foreach (char c in writer)
        {
            if (text.text.Length > 0)
            {
                text.text = text.text.Substring(0, text.text.Length - leadingChar.Length);
            }
            text.text += c;
            text.text += leadingChar;
            yield return new WaitForSeconds(timeBtwnChars);
        }

        if (leadingChar != "")
        {
            text.text = text.text.Substring(0, text.text.Length - leadingChar.Length);
        }
    }
}
