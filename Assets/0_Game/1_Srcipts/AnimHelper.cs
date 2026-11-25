using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class AnimHelper : MonoBehaviour
{
    public static AnimHelper Instance;

    [Header("Mana Text Animation")]
    public TextMeshProUGUI manaText;
    public float scaleMultiplier = 1.3f;
    public float duration = 0.2f;
    private Vector3 originalScale;
    private Tween manaTween;

    [Header("EnturnOutline Animation")]
    public Image enturnOutline;
    private Tween outlineTween;

    private void Awake()
    {
        Instance = this;
        if (manaText != null)
            originalScale = manaText.transform.localScale;

        if (enturnOutline != null)
            enturnOutline.enabled = false;
    }

    private void Start()
    {
        PlayEnturnOutlineAnim();
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

    public void PlayEnturnOutlineAnim()
    {
        if (enturnOutline == null) return;
        outlineTween?.Kill();
        enturnOutline.enabled = true;
        Color c = enturnOutline.color;
        c.a = 1f;
        enturnOutline.color = c;
        outlineTween = enturnOutline.DOFade(0.2f, 1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
    public void StopEnturnOutlineAnim(bool disableImage = true)
    {
        outlineTween?.Kill();

        if (enturnOutline != null)
        {
            if (disableImage)
            {
                enturnOutline.enabled = false;
            }
            else
            {
                Color c = enturnOutline.color;
                c.a = 1f;
                enturnOutline.color = c;
            }
        }
    }

    private void OnDestroy()
    {
        manaTween?.Kill();
        outlineTween?.Kill();
    }

}