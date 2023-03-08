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
    Queue<string> sentencesToType;

    public bool isTyping { get; private set; } = false;

    float nextDialogueDelay = 1;
    float elaspedDelayTime = 0;
    bool delayActive;

    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;

        sentences = new Queue<string>();
        sentencesToType = new Queue<string>();
    }

    private void Update()
    {
        if (!isTyping && sentencesToType.Count > 0 && !delayActive)
        {
            BeginTextDelay();
        }

        if (delayActive)
        {
            if (elaspedDelayTime >= nextDialogueDelay)
            {
                DisplayQueuedSentence();

                delayActive = false;
            }

            elaspedDelayTime += Time.deltaTime;
        }
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
        if (!isTyping)
        {
            if (sentences.Count == 0)
            {
                EndDialogue();

                return;
            }

            string currentSentence = sentences.Dequeue();

            StartCoroutine(TypeSentence(currentSentence));
        }
    }

    public void DisplayQueuedSentence()
    {
        if (!isTyping)
        {
            StartCoroutine(TypeSentence(sentencesToType.Dequeue()));
        }
    }

    public void QueueNextSentence()
    {
        sentencesToType.Enqueue(sentences.Dequeue());
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

    void BeginTextDelay()
    {
        delayActive = true;
        elaspedDelayTime = 0;
    }
}
