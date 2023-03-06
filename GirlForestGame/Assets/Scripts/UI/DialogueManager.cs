using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogueText;

    [Range(1, 5)]
    [SerializeField] float textSpeed;

    Queue<string> sentences;

    public bool isTyping { get; private set; } = false;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;

        sentences = new Queue<string>();
    }

    private void Update()
    {
        
    }

    public void StartDialogue(Dialogue dialogue)
    {
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        //if (!isTyping)
        //{
            if (sentences.Count == 0)
            {
                EndDialogue();

                return;
            }

            string currentSentence = sentences.Dequeue();

            StartCoroutine(TypeSentence(currentSentence));
        //}
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;

        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(0.1f / textSpeed);
            //yield return null;
        }

        isTyping = false;
        yield return null;
    }

    void EndDialogue()
    {
        print("Convo Ended");
    }
}
