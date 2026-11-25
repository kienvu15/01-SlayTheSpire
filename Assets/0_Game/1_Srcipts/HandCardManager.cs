using System.Collections.Generic;
using UnityEngine;

public class HandCardManager : MonoBehaviour
{
    [Header("References")]
    public HandLayout handLayout;

    private List<GameObject> handCards = new List<GameObject>();

    private void Awake()
    {
        if (handLayout == null)
            handLayout = GetComponent<HandLayout>();

        RefreshCards();
    }

    public void RefreshCards()
    {
        handCards.Clear();

        foreach (Transform child in transform)
        {
            if (child.GetComponent<CardDisplay>() != null)
                handCards.Add(child.gameObject);
        }
    }

    public void SelectCard(GameObject card)
    {
        if (handLayout == null) return;

        RefreshCards();
        handLayout.SelectCard(handCards, card);
    }

    public void ClearSelection()
    {
        if (handLayout == null) return;

        handLayout.ClearSelection();
    }
}