using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NewShopSystem : MonoBehaviour
{
    [Header("Helper")]
    public CardCollection_OnRun cardCollection_OnRun;
    public float shopSelectionScale = 1.05f;
    public float shopSelectionDuration = 0.18f;
    private Dictionary<Transform, Vector3> originalShopScales = new();

    [Header("UI_Helper")]
    public TextMeshProUGUI goldText;
    public Button Enturn_BuyButton;
    public RectTransform Player_Info;

    [Header("Data_References")]
    public AllCardData allCardDatabase;
    public CardHolder playerCardHolder;
    public Transform shopSlotsParent3;
    public Transform shopSlotsParent2;
    public GameObject cardDisplayPrefab;
    private List<CardData> shopCards = new List<CardData>();

    [Header("Relics Shop")]
    public AllRelicDatabase allRelicDatabase;
    public Transform relicSlotsParent;
    public GameObject relicDisplayPrefab;
    private List<Relic> shopRelics = new List<Relic>();
    public RelicManager relicManager;


    private CardDisplay currentSelectedCardDisplay;
    private RelicDisplay currentSelectedRelicDisplay;
    public List<RelicDisplay> spawnedRelicDisplays = new List<RelicDisplay>();

    void Start()
    {
        GenerateShop();
        GenerateRelicShop();
    }

    public void GenerateShop()
    {
        foreach (Transform child in shopSlotsParent3)
            Destroy(child.gameObject);

        foreach (Transform child in shopSlotsParent2)
            Destroy(child.gameObject);

        shopCards.Clear();
        currentSelectedCardDisplay = null; 

        for (int i = 0; i < 3; i++)
        {
            CardData randomCard = allCardDatabase.GetRandomCard();
            if (randomCard == null) continue;
            shopCards.Add(randomCard);
            GameObject slot = Instantiate(cardDisplayPrefab, shopSlotsParent3);

            CardDisplay display = slot.GetComponent<CardDisplay>();
            display.gameObject.GetComponent<Canvas>().overrideSorting = false;
            display.card_Button.onClick.AddListener(() => OnCard_Clicked(display, randomCard));
            InitShop(display, randomCard, this);
        }

        for (int i = 0; i < 2; i++)
        {
            CardData randomCard = allCardDatabase.GetRandomCard();
            if (randomCard == null) continue;
            shopCards.Add(randomCard);
            GameObject slot = Instantiate(cardDisplayPrefab, shopSlotsParent2);

            CardDisplay display = slot.GetComponent<CardDisplay>();
            display.gameObject.GetComponent<Canvas>().overrideSorting = false;
            display.card_Button.onClick.AddListener(() => OnCard_Clicked(display, randomCard));
            InitShop(display, randomCard, this);
        }
    }

    public void OnCard_Clicked(CardDisplay pickedCard, CardData data)
    {
        SelectCard(pickedCard);

        UIManager.Instance.CardInfo_Help(true);
        cardCollection_OnRun.ShowConditions(pickedCard.cardInstance);
        UIManager.Instance.Init_CardInfor(pickedCard, data);
    }

    private void SetShopSelectionVisual(
    Transform target,
    bool selected,
    GameObject outlineGlow = null)
    {
        if (target == null) return;

        if (!originalShopScales.ContainsKey(target))
            originalShopScales.Add(target, target.localScale);

        target.DOKill();

        Vector3 originalScale = originalShopScales[target];

        Vector3 targetScale = selected
            ? originalScale * shopSelectionScale
            : originalScale;

        target.DOScale(targetScale, shopSelectionDuration)
            .SetEase(Ease.OutCubic);

        if (outlineGlow != null)
            outlineGlow.SetActive(selected);
    }

    public void InitShop(CardDisplay display, CardData data, NewShopSystem shop)
    {
        display.LoadCard(data);
    }

    public void SelectCard(CardDisplay cardDisplay)
    {
        if (cardDisplay == null) return;
        // Click lại card đang chọn -> bỏ chọn
        if (currentSelectedCardDisplay == cardDisplay)
        {
            DeselectCurrentCard();
            return;
        }

        // Trả card cũ về trạng thái ban đầu
        if (currentSelectedCardDisplay != null)
        {
            SetShopSelectionVisual(
                currentSelectedCardDisplay.transform,
                false,
                currentSelectedCardDisplay.outline_Glow
            );
        }
        currentSelectedCardDisplay = cardDisplay;
        // Hiệu ứng card mới
        SetShopSelectionVisual(
            cardDisplay.transform,
            true,
            cardDisplay.outline_Glow
        );

        Enturn_BuyButton.onClick.AddListener(() => BuySelectedCard());
        Debug.Log("Đã chọn mua: " + cardDisplay.cardData.cardName);
    }

    // Button Helper
    public void BuySelectedCard()
    {
        if (currentSelectedCardDisplay == null)
        {
            Debug.Log("Chưa chọn card để mua!");
            return;
        }

        CardDisplay selectedDisplay = currentSelectedCardDisplay;
        CardData cardToBuy = selectedDisplay.cardData;

        if (cardToBuy == null || playerCardHolder == null)
            return;

        int cost = cardToBuy.goldCost;

        if (CoinManager.Instance == null ||
            !CoinManager.Instance.SpendCoins(cost))
        {
            Debug.Log("Không đủ vàng!");
            return;
        }

        SoundManager.Instance.Play("Coin");

        playerCardHolder.AddCard(cardToBuy);

        shopCards.Remove(cardToBuy);

        // Khóa lựa chọn để tránh mua liên tục
        currentSelectedCardDisplay = null;

        SetShopSelectionVisual(
            selectedDisplay.transform,
            false,
            selectedDisplay.outline_Glow
        );

        Debug.Log("Mua thành công: " + cardToBuy.cardName);

        UIManager.Instance.AnimateCardFly(
            selectedDisplay.GetComponent<RectTransform>(),
            Player_Info
        );
    }

    public void DeselectCurrentCard()
    {
        if (currentSelectedCardDisplay != null)
        {
            currentSelectedCardDisplay = null;
        }
        Debug.Log("Đã bỏ chọn");
    }

    public void GenerateRelicShop()
    {
        // Xoá relic shop cũ
        foreach (Transform child in relicSlotsParent)
            Destroy(child.gameObject);

        shopRelics.Clear();

        // Lấy toàn bộ relic có thể bán
        List<Relic> availableRelics = new List<Relic>(allRelicDatabase.allRelics);
        availableRelics.RemoveAll(relic =>
            relicManager.equippedRelics.Exists(equipped =>
                equipped != null && equipped.relicData == relic
            )
        );

        // Random tối đa 2 relic từ danh sách available (không trùng)
        for (int i = 0; i < 2; i++)
        {
            if (availableRelics.Count == 0) break;
            int index = Random.Range(0, availableRelics.Count);
            Relic randomRelic = availableRelics[index];

            shopRelics.Add(randomRelic);
            availableRelics.RemoveAt(index);

            // Tạo UI slot
            GameObject slot = Instantiate(relicDisplayPrefab, relicSlotsParent);
            RelicDisplay display = slot.GetComponent<RelicDisplay>();

            // Tạo RelicInstance
            RelicInstance instance = new RelicInstance
            {
                relicData = randomRelic,
                level = 1
            };

            display.Init_Data(instance);
            display.shopSystem = this;

            display.priceTXT.text = randomRelic.gold_Buy.ToString();

            display.goldContainer.SetActive(true);
            display.OnClickAddLisent();

            spawnedRelicDisplays.Add(display);
        }
    }

  

}
