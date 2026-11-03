using UnityEngine;

public class CardHolder : MonoBehaviour
{
    [Header("Card_Deck")]
    public Deck deck;

    private CharacterData playerData;

    public void Initialize(CharacterData data)
    {
        playerData = data;

        if (playerData == null)
        {
            Debug.LogError("CharacterData is null!");
            return;
        }

        if (playerData.startDeck == null)
        {
            Debug.LogError("Starting Deck is not assigned!");
            return;
        }

        deck.BuildDeck(playerData.startDeck.allCardData);
    }

    // Thêm card vào Draw Pile
    public void AddCard(CardData card)
    {
        if (card == null) return;

        deck.AddCard(card);
    }

    // Xóa card khỏi Draw Pile
    public void RemoveCard(CardData card)
    {
        if (card == null) return;

        deck.RemoveFromDeck(card);
    }

}