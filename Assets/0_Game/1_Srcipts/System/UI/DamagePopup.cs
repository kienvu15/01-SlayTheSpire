
using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class DamagePopup : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI textMesh;

    [Header("Animation")]
    public float moveSpeed = 180f;
    public float lifeTime = 0.5f;

    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Sequence sequence;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(
        Vector2 startPosition,
        int damageAmount,
        bool isMiss,
        Action<DamagePopup> onComplete)
    {
        sequence?.Kill();

        gameObject.SetActive(true);

        rect.anchoredPosition = startPosition;
        rect.localScale = Vector3.one;

        canvasGroup.alpha = 1f;

        textMesh.text = isMiss ? "MISS" : damageAmount.ToString();
        textMesh.color = isMiss ? Color.gray : Color.white;

        float randomX = UnityEngine.Random.Range(-0.5f, 0.5f);
        float randomY = UnityEngine.Random.Range(0.8f, 1.2f);

        Vector2 direction = new Vector2(randomX, randomY).normalized;

        Vector2 targetPosition =
            startPosition + direction * moveSpeed * lifeTime;

        sequence = DOTween.Sequence();

        sequence.Join(
            rect.DOAnchorPos(targetPosition, lifeTime)
                .SetEase(Ease.OutQuad)
        );

        sequence.Join(
            canvasGroup.DOFade(0f, lifeTime)
        );

        sequence.OnComplete(() =>
        {
            onComplete?.Invoke(this);
        });
    }

    public void ResetPopup()
    {
        sequence?.Kill();
        sequence = null;

        canvasGroup.alpha = 1f;
        rect.localScale = Vector3.one;
    }
}
