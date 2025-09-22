using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    [Header("References")]
    public CardHolder cardHolder;
    public Discard discard;
    public GameObject cardPrefab;
    public Transform handParent;
    public Transform deckTransform;   // vị trí chồng bài (spawn point)

    [Header("UI Debug")]
    public TextMeshProUGUI[] deckCountText;
    public TextMeshProUGUI discardCountText;

    [Header("Animation Settings")]
    public float drawDuration = 0.6f;   // thời gian 1 lá bay ra
    public float drawDelay = 0.15f;     // delay giữa các lá
    public float refillDelay = 3f;      // thời gian chờ trước khi refill từ discard

    private List<CardData> deck = new List<CardData>();
    private List<GameObject> currentHand = new List<GameObject>();

    void Start()
    {
        BuildDeck();
        UpdateUI();
        cardHolder = FindFirstObjectByType<CardHolder>();
    }

    public bool hasDrawFisrtRow = false;
    private void Update()
    {
        if (GameSystem.Instance != null && GameSystem.Instance.IsBattlePhase == true && hasDrawFisrtRow == false)
        {
            hasDrawFisrtRow = true;
            StartCoroutine(DelayAndDraw());
        }
    }

    public void CacheDeckCountText()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("DeckCount");
        List<TextMeshProUGUI> foundTexts = new List<TextMeshProUGUI>();

        foreach (var go in gos)
        {
            var tmp = go.GetComponent<TextMeshProUGUI>();
            if (tmp != null) foundTexts.Add(tmp);
        }

        deckCountText = foundTexts.ToArray();
        UpdateUI(); // cập nhật ngay số bài
    }

    public void CacheDeckDiscard()
    {
        discardCountText = GameObject.Find("Discard_Count")?.GetComponent<TextMeshProUGUI>();
    }

    public void RemoveCardFromHand(GameObject cardGO, Match matchRef)
    {
        if (cardGO == null) return;

        if (currentHand.Contains(cardGO))
            currentHand.Remove(cardGO);

        Destroy(cardGO);
        UpdateUI();

        //if (currentHand.Count == 0)
        //{
        //    Debug.Log("[Deck] Hand empty. Auto EndTurn.");
        //    matchRef?.EndTurn();
        //}
    }



    IEnumerator DelayAndDraw()
    {
        yield return null; // chờ 1 frame để Unity build layout
        yield return StartCoroutine(DrawHand(5));
    }

    // Copy collection vào deck và shuffle
    public void BuildDeck()
    {
        deck.Clear();
        foreach (CardData data in cardHolder.allCardData)
            deck.Add(data);
        Shuffle(deck);
        UpdateUI();
    }

    // Giữ để backward compatibility nếu nơi khác gọi
    public IEnumerator DrawHand(int count)
    {
        // đơn giản redirect sang DrawCards (vì DrawHand rút toàn bộ hand)
        yield return StartCoroutine(DrawCards(count));
    }

    /// <summary>
    /// Rút count lá (nếu deck hết giữa chừng sẽ dừng và trả về)
    /// </summary>
    public IEnumerator DrawCards(int count)
    {
        for (int k = 0; k < count; k++)
        {
            if (deck.Count == 0)
            {
                Debug.Log($"[Deck] Deck empty while drawing. Drawed so far: {k}/{count}");
                yield break;
            }

            int slotIndex = GetFirstEmptySlot();
            if (slotIndex == -1)
            {
                Debug.LogWarning("No empty slot available in hand!");
                yield break;
            }

            yield return StartCoroutine(DrawOneCard(slotIndex));
            UpdateUI();
        }
    }

    private IEnumerator DrawOneCard(int handIndex)
    {
        if (handIndex >= handParent.childCount)
        {
            Debug.LogError($"Không đủ slot trong handParent! (Cần {handIndex + 1}, chỉ có {handParent.childCount})");
            yield break;
        }

        if (deck.Count == 0)
        {
            Debug.LogWarning("[DrawOneCard] called but deck empty.");
            yield break;
        }

        // Lấy top card
        CardData drawnCard = deck[0];
        deck.RemoveAt(0);

        // Spawn card trực tiếp vào slot
        Transform targetBox = handParent.GetChild(handIndex);
        GameObject cardGO = Instantiate(cardPrefab, deckTransform.position, Quaternion.identity, targetBox);
        cardGO.GetComponent<CardDisplay>().LoadCard(drawnCard);
        currentHand.Add(cardGO);

        RectTransform rect = cardGO.GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;
        rect.localPosition = Vector3.zero;
        rect.localRotation = Quaternion.Euler(0, 90, 0);

        // Animate: bay từ deck → slot
        Sequence seq = DOTween.Sequence();
        seq.Append(rect.DOMove(targetBox.position, drawDuration).SetEase(Ease.OutQuad));
        seq.Join(rect.DOScale(Vector3.one, drawDuration).SetEase(Ease.OutBack));
        seq.Join(rect.DOLocalRotate(Vector3.zero, drawDuration));

        // Đảm bảo localPosition = 0 sau anim
        seq.OnComplete(() =>
        {
            rect.localPosition = Vector3.zero;
        });

        yield return new WaitForSeconds(drawDelay);
    }

    private int GetFirstEmptySlot()
    {
        for (int i = 0; i < handParent.childCount; i++)
        {
            if (handParent.GetChild(i).childCount == 0)
                return i;
        }
        return -1;
    }

    // Xóa và đồng thời Destroy GameObjects in hand (dùng khi cần xóa ngay)
    public void DestroyAndClearHand()
    {
        foreach (GameObject card in currentHand)
        {
            if (card != null) Destroy(card);
        }
        currentHand.Clear();
    }

    // CHỈ xóa tham chiếu trong deck (dùng khi objects đã bị destroy bởi animation khác)
    public void ClearHandReferencesOnly()
    {
        currentHand.Clear();
    }

    public List<GameObject> GetCurrentHandObjects()
    {
        return currentHand;
    }

    public int GetDeckCount()
    {
        return deck.Count;
    }

    // Nhận list cardData vào cuối deck (đáy)
    public void ReceiveCards(List<CardData> cards)
    {
        if (cards == null || cards.Count == 0) return;
        deck.AddRange(cards);
        UpdateUI();
    }

    public void ShuffleDeck()
    {
        Shuffle(deck);
        UpdateUI();
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    public void RemoveFromDeck(CardData data)
    {
        if (deck.Contains(data))
            deck.Remove(data);
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (deckCountText != null)
            foreach (var deckCountText in deckCountText)
                if (deckCountText != null)
                    deckCountText.text = deck.Count.ToString();

        if (discardCountText != null && discard != null)
            discardCountText.text = discard.Count.ToString();
    }


}
