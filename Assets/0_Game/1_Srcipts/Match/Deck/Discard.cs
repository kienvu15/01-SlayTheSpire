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
    public CanvasGroup PlayerSelfCast;

    [Header("Animation")]
    public float moveDuration = 0.5f;
    public float scaleDuration = 0.5f;
    public float delayBetweenCards = 0.1f;

    [HideInInspector]
    public List<CardInstance> discardPile = new List<CardInstance>();

    public int Count => discardPile.Count;

    // Đưa vào discard (giữ nguyên level)
    public void AddToDiscard(CardInstance card)
    {
        if (card == null) return;

        discardPile.Add(card);
        Debug.Log($"[Discard] Added {card.Name} (level {card.level}), total now {discardPile.Count}");

        if (PlayerSelfCast != null)
            PlayerSelfCast.blocksRaycasts = true;

        UpdateDiscardCount();
        PunchDiscardIcon();
    }

    // Overload cũ: code nào còn truyền CardData vẫn chạy (level 1)
    public void AddToDiscard(CardData card)
    {
        if (card == null) return;
        AddToDiscard(new CardInstance(card));
    }

    /// Lấy toàn bộ bài trong discard và xóa sạch discard
    public List<CardInstance> TakeAllCards()
    {
        List<CardInstance> taken = new List<CardInstance>(discardPile);
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

    private void PunchDiscardIcon()
    {
        if (discardTransform == null) return;

        discardTransform.DOKill();
        discardTransform.localScale = Vector3.one;
        discardTransform.DOScale(1.2f, 0.15f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                discardTransform.DOScale(1f, 0.15f).SetEase(Ease.InQuad);
            });
    }

    /// Animate hand bay về discard và Destroy card GO
    public IEnumerator AnimateDiscardHand(List<GameObject> handObjects)
    {
        foreach (GameObject cardGO in handObjects)
        {
            if (cardGO == null) continue;

            RectTransform rect = cardGO.GetComponent<RectTransform>();
            Vector3 droppedPos = rect.position;
            rect.position = droppedPos;

            Sequence seq = DOTween.Sequence();

            seq.Append(rect.DOMove(discardTransform.position, moveDuration).SetEase(Ease.InOutQuad));
            seq.Join(rect.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InBack));
            seq.Join(rect.DORotate(new Vector3(0, 0, 360), moveDuration, RotateMode.FastBeyond360));

            seq.OnComplete(() =>
            {
                Destroy(cardGO);
                PunchDiscardIcon();
            });

            yield return new WaitForSeconds(delayBetweenCards);
        }

        UpdateDiscardCount();
    }

    /// Update số lá trong discard lên UI
    private int currentDisplayCount = 0;

    public void UpdateDiscardCount()
    {
        if (discardCountText == null) return;

        int targetCount = discardPile.Count;

        DOTween.Kill(discardCountText);

        DOTween.To(() => currentDisplayCount, x =>
        {
            currentDisplayCount = x;
            discardCountText.text = "Discard Pile - " + currentDisplayCount.ToString();
        },
        targetCount, 0.5f)
        .SetEase(Ease.OutQuad)
        .SetId(discardCountText);
    }

}