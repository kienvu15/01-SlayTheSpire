using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextType : MonoBehaviour
{
    public GameObject EnterButton;
    public GameObject dialogueButton;
    public GameObject SkipShopButton;

    public TextMeshProUGUI dialogueCop;
    public string[] lines;
    public float textSpeed = 0.05f;
    private int index;
    private bool isDone;

    void Start()
    {
        dialogueCop.text = string.Empty;
        StartDialogue();
    }

    public void ButtonDialugue()
    {
        if (dialogueCop.text == lines[index])
            NextLine();
        else
        {
            StopAllCoroutines();
            dialogueCop.text = lines[index];
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            dialogueCop.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            dialogueCop.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            dialogueButton.SetActive(false);
            SkipShopButton.SetActive(true);
            EnterButton.SetActive(true);
        }
    }

    IEnumerator endDioluge()
    {
        yield return new WaitForSeconds(1.4f);
        isDone = true;
        dialogueCop.gameObject.SetActive(false);
    }
}
