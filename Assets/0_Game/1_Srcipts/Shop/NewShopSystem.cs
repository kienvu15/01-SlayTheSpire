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

    [Header("Remove_Helper")]
    public Button Remove_card_Service;
    public GameObject remove_Panel;
    public Transform removeParent;
    public float removeSelectScale = 1.15f;
    public float removeSelectDuration = 0.18f;
    public float removeDestroyDuration = 0.25f;

    private CardDisplay currentSelectedRemoveCardDisplay;
    private CardInstance selectedRemoveCardInstance;
    private Dictionary<Transform, Vector3> removeOriginalScales = new();

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
    public bool IsRelicSelected(RelicDisplay d) => currentSelectedRelicDisplay == d;

    void Start()
    {
        GenerateShop();
        GenerateRelicShop();

        Enturn_BuyButton.onClick.RemoveAllListeners();
        Enturn_BuyButton.onClick.AddListener(BuySelected);

        Remove_card_Service.onClick.AddListener(() => Open_Remove_Service());
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

        if (currentSelectedCardDisplay == null) return; // vừa bỏ chọn

        UIManager.Instance.CardInfo_Help(true);
        cardCollection_OnRun.ShowConditions(pickedCard.cardInstance);
        UIManager.Instance.Init_CardInfor(pickedCard, data);
    }

    public void InitShop(CardDisplay display, CardData data, NewShopSystem shop)
    {
        display.LoadCard(data);
    }

    public void SelectCard(CardDisplay cardDisplay)
    {
        if (cardDisplay == null) return;

        // click lại cái đang chọn -> bỏ chọn
        if (currentSelectedCardDisplay == cardDisplay)
        {
            DeselectAll();
            return;
        }

        DeselectAll(); // bỏ chọn cả card cũ lẫn relic cũ

        currentSelectedCardDisplay = cardDisplay;
        SetShopSelectionVisual(cardDisplay.transform, true, cardDisplay.outline_Glow);
    }

    // ---------- RELIC ----------
    public void SelectRelic(RelicDisplay relicDisplay)
    {
        if (relicDisplay == null) return;

        if (currentSelectedRelicDisplay == relicDisplay)
        {
            DeselectAll();
            return;
        }

        DeselectAll(); // bỏ chọn card đang chọn (nếu có)

        currentSelectedRelicDisplay = relicDisplay;
        relicDisplay.SetSelected(true);
    }

    // ---------- CHUNG ----------
    public void DeselectAll()
    {
        if (currentSelectedCardDisplay != null)
        {
            SetShopSelectionVisual(
                currentSelectedCardDisplay.transform,
                false,
                currentSelectedCardDisplay.outline_Glow);
            currentSelectedCardDisplay = null;
        }

        if (currentSelectedRelicDisplay != null)
        {
            currentSelectedRelicDisplay.SetSelected(false);
            currentSelectedRelicDisplay = null;
        }
    }

    private void SetShopSelectionVisual(Transform target, bool selected, GameObject outlineGlow = null)
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

    public void BuySelected()
    {
        if (currentSelectedCardDisplay != null)
            BuySelectedCard();
        else if (currentSelectedRelicDisplay != null)
            BuySelectedRelic();
        else
            Debug.Log("Chưa chọn gì để mua!");
    }

    public void BuySelectedRelic()
    {
        var display = currentSelectedRelicDisplay;
        if (display == null) return;

        Relic relic = display.RelicData;
        if (CoinManager.Instance == null || !CoinManager.Instance.SpendCoins(relic.gold_Buy))
        {
            Debug.Log("Không đủ vàng!");
            return;
        }

        SoundManager.Instance.Play("Coin");

        relicManager.AddRelic(relic);

        shopRelics.Remove(relic);
        DeselectAll();
        display.SetSoldOut();
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
            display.OnClickAddLisentShop();

            spawnedRelicDisplays.Add(display);
        }
    }

    //UI Helper
    public void Open_Remove_Service()
    {
        remove_Panel.SetActive(true);

        removeOriginalScales.Clear(); // card cũ bị rebuild nên bỏ scale cũ
        currentSelectedRemoveCardDisplay = null;
        selectedRemoveCardInstance = null;

        cardCollection_OnRun.RefreshPanel(removeParent, OnRemoveCardClicked);

        Enturn_BuyButton.onClick.RemoveAllListeners();
        Enturn_BuyButton.onClick.AddListener(ConfirmRemoveCard);

        TextMeshProUGUI btnText = Enturn_BuyButton.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null)
            btnText.text = "Remove";
    }

    public void OnRemoveCardClicked(CardDisplay display)
    {
        if (display == null || display.cardInstance == null) return;

        // bấm lại lá đang chọn -> bỏ chọn
        if (currentSelectedRemoveCardDisplay == display)
        {
            DeselectRemoveCard();
            return;
        }

        DeselectRemoveCard(); // thu nhỏ lá cũ

        currentSelectedRemoveCardDisplay = display;
        selectedRemoveCardInstance = display.cardInstance;
        SetRemoveSelectionVisual(display.transform, true, display.outline_Glow);
    }

    private void DeselectRemoveCard()
    {
        if (currentSelectedRemoveCardDisplay != null)
        {
            SetRemoveSelectionVisual(
                currentSelectedRemoveCardDisplay.transform,
                false,
                currentSelectedRemoveCardDisplay.outline_Glow);
        }
        currentSelectedRemoveCardDisplay = null;
        selectedRemoveCardInstance = null;
    }

    private void SetRemoveSelectionVisual(Transform target, bool selected, GameObject outlineGlow = null)
    {
        if (target == null) return;

        if (!removeOriginalScales.ContainsKey(target))
            removeOriginalScales.Add(target, target.localScale);

        Vector3 original = removeOriginalScales[target];

        target.DOKill();
        target.DOScale(selected ? original * removeSelectScale : original, removeSelectDuration)
              .SetEase(Ease.OutCubic)
              .SetLink(target.gameObject);

        if (outlineGlow != null)
            outlineGlow.SetActive(selected);
    }

    public void ConfirmRemoveCard()
    {
        if (selectedRemoveCardInstance == null ||
            currentSelectedRemoveCardDisplay == null ||
            cardCollection_OnRun.deck == null)
        {
            Debug.Log("Chưa chọn lá bài nào để xóa!");
            return;
        }

        CardInstance cardToRemove = selectedRemoveCardInstance;
        CardDisplay displayToDestroy = currentSelectedRemoveCardDisplay;

        // Xóa khỏi toàn bộ bộ bài (allCards + draw pile)
        cardCollection_OnRun.deck.RemoveCardFromCollection(cardToRemove);

        Debug.Log("Đã xóa thành công lá bài: " + cardToRemove.data.cardName);

        // Reset lựa chọn trước, tránh bấm Remove nhiều lần lên cùng 1 lá
        currentSelectedRemoveCardDisplay = null;
        selectedRemoveCardInstance = null;
        removeOriginalScales.Remove(displayToDestroy.transform);

        // Destroy card trong panel (thu nhỏ rồi xóa)
        if (displayToDestroy.card_Button != null)
            displayToDestroy.card_Button.interactable = false;

        GameObject go = displayToDestroy.gameObject;
        go.transform.DOKill();
        go.transform.DOScale(Vector3.zero, removeDestroyDuration)
            .SetEase(Ease.InBack)
            .SetLink(go)
            .OnComplete(() => Destroy(go));
    }

}
