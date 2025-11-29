using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewShopSystem : MonoBehaviour
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

    [Header("UI")]
    public RectTransform buyButtonRect;
    [HideInInspector] public Vector2 buyButtonOriginalPos;
    public RectTransform bgImageRect;
    [HideInInspector] public Vector2 bgImageOriginalPos;
    public RectTransform leaveButtonRect;
    [HideInInspector] public Vector2 leaveButtonOriginalPos;
    public RectTransform dialogBoxRect;
    [HideInInspector] public Vector2 dialogBoxOriginalPos;
    public CanvasGroup shopOusideBlocker;

    public Button buyButton;

    private CardDisplay currentSelectedCardDisplay;
    private RelicDisplay currentSelectedRelicDisplay;

    private void OnEnable()
    {
        buyButtonRect.anchoredPosition = new Vector2(-1215, -96);
        bgImageRect.anchoredPosition = new Vector2(121, 66);
        leaveButtonRect.anchoredPosition = new Vector2(-952, -256);
        dialogBoxRect.anchoredPosition = new Vector2(255, -342);
    }

    void Start()
    {
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuySelectedCard);
            buyButtonOriginalPos = buyButtonRect.anchoredPosition;
            bgImageOriginalPos = bgImageRect.anchoredPosition;
        }

        GenerateShop();
        GenerateRelicShop();
    }

    public void GenerateShop()
    {
        foreach (Transform child in shopSlotsParent)
            Destroy(child.gameObject);

        shopCards.Clear();
        currentSelectedCardDisplay = null; 

        for (int i = 0; i < 5; i++)
        {
            CardData randomCard = allCardDatabase.GetRandomCard();
            if (randomCard == null) continue;
            shopCards.Add(randomCard);
            GameObject slot = Instantiate(cardDisplayPrefab, shopSlotsParent);
            CardDisplay display = slot.GetComponent<CardDisplay>();
            display.useScaleAnimation = true;
            display.InitShop(randomCard, this);
        }
    }

    public void SelectCard(CardDisplay cardDisplay)
    {
        if (cardDisplay == null) return;

        // Nếu đang có relic mở → đóng
        if (currentSelectedRelicDisplay != null)
        {
            currentSelectedRelicDisplay.HideDescription();
            currentSelectedRelicDisplay = null;
        }

        // Nếu bấm lại card đang chọn → bỏ chọn
        if (currentSelectedCardDisplay == cardDisplay)
        {
            cardDisplay.HideDescription();
            DeselectCurrentCard();
            return;
        }

        // Nếu đang có card khác → đóng
        if (currentSelectedCardDisplay != null)
            currentSelectedCardDisplay.HideDescription();

        currentSelectedCardDisplay = cardDisplay;
        cardDisplay.ToggleDescription();

        // Move nút Buy
        if (buttonMoveCoroutine != null) StopCoroutine(buttonMoveCoroutine);
        buyButtonRect.anchoredPosition = buyButtonOriginalPos;
        buttonMoveCoroutine = StartCoroutine(ButtonMovetoRight());

        Debug.Log("Đã chọn mua: " + cardDisplay.cardData.cardName);
    }


    public void DeselectCurrentCard()
    {
        if (currentSelectedCardDisplay != null)
        {
            currentSelectedCardDisplay.HideDescription();

            currentSelectedCardDisplay = null;
        }

        if (buyButtonRect != null)
        {
            if (buttonMoveCoroutine != null) StopCoroutine(buttonMoveCoroutine);
            buttonMoveCoroutine = StartCoroutine(ButtonMovetoLeft()); 
        }

        Debug.Log("Đã bỏ chọn");
    }

    private Coroutine buttonMoveCoroutine;

    public IEnumerator ButtonMovetoRight()
    {
        Vector2 startPos = buyButtonRect.anchoredPosition;
        Vector2 targetPos = startPos + new Vector2(325f, 0f);

        float duration = 0.4f;
        float elapsed = 0f;

        //SoundManager.Instance.Play("RockMove");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            buyButtonRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        buyButtonRect.anchoredPosition = targetPos;
    }

    public IEnumerator ButtonMovetoLeft()
    {
        Vector2 startPos = buyButtonRect.anchoredPosition;
        Vector2 targetPos = startPos - new Vector2(325f, 0f);

        float duration = 0.4f;
        float elapsed = 0f;

        //SoundManager.Instance.Play("RockMove");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            buyButtonRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        buyButtonRect.anchoredPosition = targetPos;
    }

    public void BuySelectedCard()
    {

        if (currentSelectedCardDisplay == null) return;
        CardData cardToBuy = currentSelectedCardDisplay.cardData;
        int cost = cardToBuy.goldCost;
        if (CoinManager.Instance != null && CoinManager.Instance.SpendCoins(cost))
        {
            SoundManager.Instance.Play("Coin");
            playerCardHolder.AddCard(cardToBuy);
            ConfirmPickReward();
        }
    }

    private void ConfirmPickReward()
    {
        if (playerCardHolder != null)
        {

            if (RandomCardSystem.Instance != null)
            {
            }

            RectTransform target = UIManager.Instance.deckIcon;

            UIManager.Instance.AnimateCardFly(
                currentSelectedCardDisplay.GetComponent<RectTransform>(),
                target,
                () => {
                    StartCoroutine(OnCardSelected(currentSelectedCardDisplay));
                    DeselectCurrentCard();

                }
            );
            for (int i = 0; i < currentSelectedCardDisplay.transform.childCount; i++)
            {
                currentSelectedCardDisplay.transform.GetChild(i).gameObject.SetActive(false);
            }

            currentSelectedCardDisplay.HideDescription();
        }
    }

    public IEnumerator OnCardSelected(CardDisplay pickedCard)
    {
        currentSelectedCardDisplay.goldContainer.SetActive(false);
        UIManager.Instance.AnimateCardFly(
           pickedCard.GetComponent<RectTransform>(),
           UIManager.Instance.deckIcon, 
           () =>
           {
               currentSelectedCardDisplay = null;

           });
        yield return new WaitForSeconds(0.3f);
    }


    public List<RelicDisplay> spawnedRelicDisplays = new List<RelicDisplay>();
    public void GenerateRelicShop()
    {
        // Xoá relic shop cũ
        foreach (Transform child in relicSlotsParent)
            Destroy(child.gameObject);

        shopRelics.Clear();

        // Lấy toàn bộ relic có thể bán
        List<Relic> availableRelics = new List<Relic>(allRelicDatabase.allRelics);

        // Loại bỏ relic đã trang bị
        foreach (var equipped in relicManager.equippedRelics)
            availableRelics.Remove(equipped);

        // Random tối đa 2 relic từ danh sách available (không trùng)
        for (int i = 0; i < 2; i++)
        {
            if (availableRelics.Count == 0) break;
            int index = Random.Range(0, availableRelics.Count);
            Relic randomRelic = availableRelics[index];

            shopRelics.Add(randomRelic);

            // Bỏ relic này khỏi danh sách để không trùng
            availableRelics.RemoveAt(index);

            // Tạo UI slot
            GameObject slot = Instantiate(relicDisplayPrefab, relicSlotsParent);
            RelicDisplay display = slot.GetComponent<RelicDisplay>();
            display.Init(randomRelic, this);
            spawnedRelicDisplays.Add(display);
        }
    }

    // Trong NewShopSystem.cs

    public void SelectRelic(RelicDisplay relicDisplay)
    {
        if (relicDisplay == null) return;

        // 1. Đóng Card đang mở (nếu có)
        if (currentSelectedCardDisplay != null)
        {
            currentSelectedCardDisplay.HideDescription();
            DeselectCurrentCard();
        }

        // 2. Nếu bấm vào chính Relic đang mở -> Đóng lại và hủy chọn
        if (currentSelectedRelicDisplay == relicDisplay)
        {
            relicDisplay.HideDescription();
            currentSelectedRelicDisplay = null;
            return;
        }

        // 3. Nếu đang mở một Relic KHÁC -> Đóng cái cũ đi
        if (currentSelectedRelicDisplay != null)
        {
            currentSelectedRelicDisplay.HideDescription();
        }

        // 4. Mở cái mới
        currentSelectedRelicDisplay = relicDisplay;

        // Đảm bảo bật lên (Thay vì dùng Toggle, ta ép nó Show luôn để tránh lỗi state)
        // Nhưng vì hàm bên kia là ToggleDescription, ta cần đảm bảo nó đang tắt trước khi gọi, 
        // hoặc gọi hàm Show riêng. Ở đây dùng Toggle cũng được vì ta đã Hide cái cũ rồi.
        relicDisplay.ToggleDescription();
    }



    public void BuyCard(CardData card)
    {
        if (card == null) return;
        playerCardHolder.AddCard(card);
        shopCards.Remove(card); // Loại bỏ khỏi danh sách shopCards
        Debug.Log("Mua thành công: " + card.cardName);

        // Ở đây bạn có thể thêm logic trừ tiền, v.v.
    }

    public void BuyRelic(Relic relic)
    {
        if (relic == null) return;
        relicManager.AddRelic(relic);
        Debug.Log("Mua thành công relic: " + relic.relicName);
    }

}
