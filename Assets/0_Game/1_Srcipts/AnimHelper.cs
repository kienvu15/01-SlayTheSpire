using UnityEngine;
using TMPro;
using DG.Tweening;

public class AnimHelper : MonoBehaviour
{
    public static AnimHelper Instance;

    [Header("Mana Text Animation")]
    public TextMeshProUGUI manaText;
    public float scaleMultiplier = 1.3f;
    public float duration = 0.2f;

    private Vector3 originalScale;
    private Tween manaTween;

    private void Awake()
    {
        Instance = this;
        if (manaText != null)
            originalScale = manaText.transform.localScale;
    }

    public void PlayManaScale()
    {
        if (manaText == null) return;

        manaTween?.Kill();

        manaText.transform.localScale = originalScale;

        manaTween = manaText.transform
            .DOScale(originalScale * scaleMultiplier, duration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                manaText.transform
                    .DOScale(originalScale, duration)
                    .SetEase(Ease.OutBack);
            });
    }

    private void OnDestroy()
    {
        manaTween?.Kill();
    }

}