using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum CardPileType
{
    DrawPile,
    DiscardPile
}

public class CardPileViewer : MonoBehaviour
{
    [Header("References")]
    public Deck deck;
    public GameObject cardDisplayPrefab;
    public Transform contentParent;
    public CenterScaleScroll scroller;

    [Header("Select Mode")]
    public Button chooseButton;               // nút "Chọn" duy nhất
    public TMPro.TMP_Text selectHintText;     // tuỳ chọn: "Chọn 1 lá"

    private CardPileType currentPile = CardPileType.DrawPile;

    private readonly List<GameObject> spawnedCards = new();
    private readonly Dictionary<CardInstance, GameObject> objectByCard = new();

    // --- trạng thái chọn ---
    private bool selectMode;
    private int remainingPicks;
    private System.Action<List<CardInstance>> onPicked;
    private readonly List<CardInstance> picked = new();

    public CardData GetSelectedCardData()
    {
        return scroller != null ? scroller.GetHighlightedCardData() : null;
    }

    // Lá đang được scale to nhất ở giữa
    public CardInstance GetHighlightedCardInstance()
    {
        if (scroller == null) return null;

        CardDisplay display = scroller.GetHighlightedCard();
        return display != null ? display.cardInstance : null;
    }

    // ================= SWITCH PILE (chỉ xem) =================
    public void ShowDrawPile()
    {
        selectMode = false;
        gameObject.SetActive(true);
        currentPile = CardPileType.DrawPile;
        RefreshPanel();
    }

    public void ShowDiscardPile()
    {
        selectMode = false;
        gameObject.SetActive(true);
        currentPile = CardPileType.DiscardPile;
        RefreshPanel();
    }

    // ================= SELECT MODE =================
    public void BeginSelect(CardPileType pile, int count,
                            System.Action<List<CardInstance>> callback)
    {
        selectMode = true;
        onPicked = callback;
        picked.Clear();
        currentPile = pile;

        UIManager.Instance.PlayerHand(false);
        gameObject.SetActive(true);
        UIManager.Instance.LeftHand_Content_Help(true, false);
        UIManager.Instance.Context(true, false);
        UIManager.Instance.default_Disciption.GetComponent<TextMeshProUGUI>().text = "Choose your path".ToString();

        Button chooseButton = UIManager.Instance.endTurn_Button;
        chooseButton.onClick.AddListener(()=>OnClickChoose());
        chooseButton.GetComponentInChildren<TextMeshProUGUI>().text = "Choose".ToString();

        RefreshPanel();

        remainingPicks = Mathf.Min(count, spawnedCards.Count);
        UpdateSelectUI();
    }

    // Gắn hàm này vào onClick của chooseButton trong Inspector
    public void OnClickChoose()
    {
        if (!selectMode) return;

        CardInstance card = GetHighlightedCardInstance();
        if (card == null) return;

        picked.Add(card);
        remainingPicks--;

        if (remainingPicks <= 0)
        {
            FinishSelect();
            return;
        }

        // Chọn nhiều lá: bỏ lá vừa chọn khỏi danh sách để chọn tiếp
        RemoveCardFromView(card);
        UpdateSelectUI();
    }

    private void RemoveCardFromView(CardInstance card)
    {
        if (objectByCard.TryGetValue(card, out GameObject obj) && obj != null)
        {
            obj.transform.SetParent(null, false);   // rời layout ngay lập tức
            spawnedCards.Remove(obj);
            Destroy(obj);
        }
        objectByCard.Remove(card);

        if (scroller != null) scroller.Refresh();
    }

    private void FinishSelect()
    {
        var result = new List<CardInstance>(picked);
        var callback = onPicked;

        selectMode = false;
        onPicked = null;
        picked.Clear();

        UIManager.Instance.LeftHand_Content(true, false);
        UIManager.Instance.Context(false, false);

        UIManager.Instance.endTurn_Button.GetComponentInChildren<TextMeshProUGUI>().text = "END TURN".ToString();
        UIManager.Instance.EndTurn_OnClick();

        UIManager.Instance.PlayerHand(true);

        UpdateSelectUI();
        gameObject.SetActive(false);

        callback?.Invoke(result);
    }

    private void UpdateSelectUI()
    {
        if (chooseButton != null)
            chooseButton.gameObject.SetActive(selectMode);

        if (selectHintText != null)
            selectHintText.text = selectMode ? $"Chọn {remainingPicks} lá" : "";
    }

    // ================= REFRESH =================
    public void RefreshPanel()
    {
        ClearCards();

        if (deck == null || contentParent == null || cardDisplayPrefab == null)
            return;

        List<CardInstance> cards = currentPile == CardPileType.DrawPile
            ? deck.GetDrawPileCards()
            : deck.GetDiscardPileCards();

        foreach (CardInstance instance in cards)
        {
            if (instance == null) continue;

            GameObject obj = Instantiate(cardDisplayPrefab, contentParent, false);
            CardDisplay display = obj.GetComponent<CardDisplay>();
            if (display == null)
            {
                Destroy(obj);
                continue;
            }

            display.currentMode = CardMode.ViewOnly;
            display.LoadCard(instance);

            DraggableCard draggable = obj.GetComponent<DraggableCard>();
            if (draggable != null) draggable.enabled = false;

            Button cardButton = obj.GetComponentInChildren<Button>(true);
            if (cardButton != null) cardButton.interactable = false;

            Canvas canvas = obj.GetComponentInChildren<Canvas>();
            if (canvas != null) canvas.overrideSorting = false;

            objectByCard[instance] = obj;
            spawnedCards.Add(obj);
        }

        if (scroller != null) scroller.Refresh();   // gọi một lần sau vòng lặp
        UpdateSelectUI();
    }

    // ================= CLEAR =================
    private void ClearCards()
    {
        foreach (GameObject obj in spawnedCards)
        {
            if (obj == null) continue;
            obj.transform.SetParent(null, false);
            Destroy(obj);
        }
        spawnedCards.Clear();
        objectByCard.Clear();
    }

    public void RefreshCurrentPile()
    {
        RefreshPanel();
    }

}