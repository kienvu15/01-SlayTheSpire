using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TextType : MonoBehaviour
{
    public TextMeshProUGUI dialogueCop;
    public string[] lines;
    public float textSpeed = 0.05f;
    private int index;
    private bool isDone;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueCop.text = string.Empty;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (dialogueCop.text == lines[index])
                NextLine();
            else
            {
                StopAllCoroutines();
                dialogueCop.text = lines[index];
            }
        }

        //if (dialogueCop.text == lines[1] && !isDone)
        //{
        //    StartCoroutine(endDioluge());
        //}
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
    }

    IEnumerator endDioluge()
    {
        yield return new WaitForSeconds(1.4f);
        isDone = true;
        dialogueCop.gameObject.SetActive(false);
    }
}
