using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllCardData", menuName = "Cards/AllCardData")]
public class AllCardData : ScriptableObject
{
    public List<CardData> allCards = new List<CardData>();

    public CardData GetRandomCard()
    {
        if (allCards.Count == 0) return null;
        int index = Random.Range(0, allCards.Count);
        return allCards[index];
    }
}
