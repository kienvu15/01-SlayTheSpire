using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    [Header("References")]
    public AllCardData allCardDatabase;    
    public CardHolder playerCardHolder;    
    public Transform shopSlotsParent;     
    public GameObject cardDisplayPrefab;   
    private List<CardData> shopCards = new List<CardData>();

    [Header("Relics Shop")]
    public AllRelicDatabase allRelicDatabase;   
    public Transform relicSlotsParent;         
    public GameObject relicDisplayPrefab;
    private List<Relic> shopRelics = new List<Relic>();
    public RelicManager relicManager;

    [Header("FF")]
    public ToggleUI deckToggleUI;
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
        //GenerateShop();
       // GenerateRelicShop();
    }

    //public void GenerateShop()
    //{
    //    // Xoá shop cũ
    //    foreach (Transform child in shopSlotsParent)
    //        Destroy(child.gameObject);

    //    shopCards.Clear();

    //    // Random 3 lá
    //    for (int i = 0; i < 3; i++)
    //    {
    //        CardData randomCard = allCardDatabase.GetRandomCard();
    //        if (randomCard == null) continue;

    //        shopCards.Add(randomCard);

    //        GameObject slot = Instantiate(cardDisplayPrefab, shopSlotsParent);
    //        CardDisplay display = slot.GetComponent<CardDisplay>();
    //        display.LoadCard(randomCard);

    //        display.Init(randomCard, this);
    //    }
    //}

    //public void GenerateRelicShop()
    //{
    //    // Xoá relic shop cũ
    //    foreach (Transform child in relicSlotsParent)
    //        Destroy(child.gameObject);

    //    shopRelics.Clear();

    //    // Lấy toàn bộ relic có thể bán
    //    List<Relic> availableRelics = new List<Relic>(allRelicDatabase.allRelics);

    //    // Loại bỏ relic đã trang bị
    //    foreach (var equipped in relicManager.equippedRelics)
    //        availableRelics.Remove(equipped);

    //    // Random tối đa 2 relic từ danh sách available (không trùng)
    //    for (int i = 0; i < 2; i++)
    //    {
    //        if (availableRelics.Count == 0) break;

    //        int index = Random.Range(0, availableRelics.Count);
    //        Relic randomRelic = availableRelics[index];

    //        shopRelics.Add(randomRelic);

    //        // Bỏ relic này khỏi danh sách để không trùng
    //        availableRelics.RemoveAt(index);

    //        // Tạo UI slot
    //        GameObject slot = Instantiate(relicDisplayPrefab, relicSlotsParent);
    //        RelicDisplay display = slot.GetComponent<RelicDisplay>();
    //        display.Init(randomRelic, this); // shopMode = true
    //    }
    //}


    public void BuyCard(CardData card) 
    { 
        if (card == null) return; 
        playerCardHolder.AddCard(card); 
        Debug.Log("Mua thành công: " + card.cardName); 
    }

    public void BuyRelic(Relic relic)
    {
        if (relic == null) return;
        relicManager.AddRelic(relic);
        Debug.Log("Mua thành công relic: " + relic.relicName);
    }



}
