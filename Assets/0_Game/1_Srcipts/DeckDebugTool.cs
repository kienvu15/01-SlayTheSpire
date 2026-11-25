using NaughtyAttributes;
using UnityEngine;
using System.Collections;

public class DeckDebugTool : MonoBehaviour
{
    public Deck deck;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            StartCoroutine(DrawFiveCards());

        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(DrawOneCard());

        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(DiscardAll());
    }

    [Button("Draw Five Cards")]
    private void DrawFiveCardsButton()
    {
        if (!Application.isPlaying) return;
        if (deck == null) return;

        StartCoroutine(DrawFiveCards());
    }

    [Button("EX Five Cards")]
    private void DiscardAllButton()
    {
        if (!Application.isPlaying) return;
        if (deck == null) return;

        StartCoroutine(DiscardAll());
    }

    [Button("EADDX Five Cards")]
    private void DrawOneCardButton()
    {
        if (!Application.isPlaying) return;
        if (deck == null) return;

        StartCoroutine(DrawOneCard());
    }

    private IEnumerator DrawFiveCards()
    {
        yield return StartCoroutine(deck.DrawHand(5));
    }

    private IEnumerator DrawOneCard()
    {
        yield return StartCoroutine(deck.DrawHand(1));
    }

    private IEnumerator DiscardAll()
    {
        yield return StartCoroutine(deck.AnimateClearHand());
    }
}