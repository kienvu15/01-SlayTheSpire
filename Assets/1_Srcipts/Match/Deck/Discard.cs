using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Discard : MonoBehaviour
{
    [Header("References")]
    public Transform discardTransform;          
    public TextMeshProUGUI discardCountText;     

    //public GameObject SystemCanvas;
    [HideInInspector] public CanvasGroup PlayerSelfCast;

    [Header("Animation")]
    public float moveDuration = 0.5f;            
    public float scaleDuration = 0.5f;           
    public float delayBetweenCards = 0.1f;       

    [HideInInspector]
    public List<CardData> discardPile = new List<CardData>();


    private void Awake()
    {
        PlayerSelfCast = GameObject.Find("PlayerSelfCast")?.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
    }

    private void Update()
    {

    }
    public int Count => discardPile.Count;

    /// Thêm lá bài vào discardPile (chỉ dữ liệu)
    public void AddToDiscard(CardData card)
    {
        if (card != null)
        {
            discardPile.Add(card);
            Debug.Log($"[Discard] Added {card.name}, total now {discardPile.Count}");
            PlayerSelfCast.blocksRaycasts = true;
            UpdateDiscardCount();
        }
    }


    /// Lấy toàn bộ bài trong discard và xóa sạch discard
    public List<CardData> TakeAllCards()
    {
        List<CardData> taken = new List<CardData>(discardPile);
        discardPile.Clear();
        UpdateDiscardCount();
        return taken;
    }

    /// Xóa sạch discard
    public void Clear()
    {
        discardPile.Clear();
        UpdateDiscardCount();
    }

    /// Animate hand bay về discard và Destroy card GO
    public IEnumerator AnimateDiscardHand(List<GameObject> handObjects)
    {
        foreach (GameObject cardGO in handObjects)
        {
            if (cardGO == null) continue;

            RectTransform rect = cardGO.GetComponent<RectTransform>();

            // Lưu vị trí bị thả (world position)
            Vector3 droppedPos = rect.position;

            // Đặt parent mới (cùng canvas với discard)
            rect.SetParent(discardTransform.parent, true);

            // Reset lại world pos = chỗ thả
            rect.position = droppedPos;

            Sequence seq = DOTween.Sequence();

            // Bay từ droppedPos -> discard
            seq.Append(rect.DOMove(discardTransform.position, moveDuration).SetEase(Ease.InOutQuad));

            // scale nhỏ dần
            seq.Join(rect.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InBack));

            // xoay vòng
            seq.Join(rect.DORotate(new Vector3(0, 0, 360), moveDuration, RotateMode.FastBeyond360));

            seq.OnComplete(() => Destroy(cardGO));

            yield return new WaitForSeconds(delayBetweenCards);
        }

        UpdateDiscardCount();
    }

    /// Update số lá trong discard lên UI
    public void UpdateDiscardCount()
    {
        if (discardCountText != null)
            discardCountText.text = discardPile.Count.ToString();
    }
}
