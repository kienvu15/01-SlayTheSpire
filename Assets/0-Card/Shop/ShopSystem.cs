using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    [Header("References")]
    public AllCardData allCardDatabase;    // database toàn bộ card
    public CardHolder playerCardHolder;    // deck của Player
    public Transform shopSlotsParent;      // nơi chứa các card UI
    public GameObject cardDisplayPrefab;   // prefab CardDisplay

    private List<CardData> shopCards = new List<CardData>();

    public ToggleUI deckToggleUI;                // cái ToggleUI để bật panel deck
    public CardCollectionPanel cardCollection;

    public void OnClickMyDeck()
    {
        cardCollection.Open(false); // removeMode = false
        deckToggleUI.PopUp();
    }

    // Nút "Remove Card" trong shop
    public void OnClickRemoveCard()
    {
        cardCollection.Open(true); // removeMode = true
        deckToggleUI.PopUp();
    }

    void Start()
    {
        GenerateShop();
    }

    public void GenerateShop()
    {
        // Xoá shop cũ
        foreach (Transform child in shopSlotsParent)
            Destroy(child.gameObject);

        shopCards.Clear();

        // Random 3 lá
        for (int i = 0; i < 3; i++)
        {
            CardData randomCard = allCardDatabase.GetRandomCard();
            if (randomCard == null) continue;

            shopCards.Add(randomCard);

            GameObject slot = Instantiate(cardDisplayPrefab, shopSlotsParent);
            CardDisplay display = slot.GetComponent<CardDisplay>();
            display.LoadCard(randomCard);

            display.Init(randomCard, this);
        }
    }

    public void BuyCard(CardData card)
    {
        if (card == null) return;

        playerCardHolder.AddCard(card);
        Debug.Log("Mua thành công: " + card.cardName);
    }
}
