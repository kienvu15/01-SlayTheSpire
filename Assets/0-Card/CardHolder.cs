using System.Collections.Generic;
using UnityEngine;

public class CardHolder : MonoBehaviour
{
    [Header("Player Collection (có thể thay đổi khi chơi)")]
    public List<CardData> allCardData = new List<CardData>();

    // Thêm 1 card vào collection
    public void AddCard(CardData card)
    {
        allCardData.Add(card);
    }

    // Xoá 1 card khỏi collection
    public void RemoveCard(CardData card)
    {
        allCardData.Remove(card);
    }
}
