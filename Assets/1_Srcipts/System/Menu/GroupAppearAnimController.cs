using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public enum AppearGroupAnimType { Scale, MoveLeft, MoveRight, MoveUp, MoveDown, Rotate }

public class GroupAppearAnimManager : MonoBehaviour
{
    [Header("Animation Groups")]
    public List<AppearAnimGroup> groups = new();
    private Dictionary<Transform, CacheData> cache = new();

    class CacheData
    {
        public Vector3 localPos;
        public Vector2 anchoredPos;
        public Vector3 scale;
        public Vector3 localRot;
        public bool isRect;
        public CanvasGroup canvasGroup;
    }

    void Awake() => CacheAll();

    void CacheAll()
    {
        foreach (var g in groups)
            foreach (var t in g.targets)
                if (t) Cache(t);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayGroupByName("Button");
            PlayGroupByName("Logo");
        }
    }

    public void PlayModeAppear()
    {
        PlayGroupByName("Mode");
        PlayGroupByName("BackButton");
        PlayGroupByName("SkinSelect");
    }

    public void MenuModeAppear()
    {
        PlayGroupByName("Logo");
        PlayGroupByName("MenuButton");
        PlayGroupByName("PlayerData");

    }

    public void PlayGroupByName(string groupName)
    {
        var g = groups.Find(x => x.groupName == groupName);
        if (g != null) Play(g);
    }

    void Play(AppearAnimGroup group)
    {
        float currentDelay = 0f;

        foreach (Transform t in group.targets)
        {
            if (!t) continue;
            
            // 1. Dọn dẹp Tween cũ trên cả Transform và CanvasGroup
            t.DOKill();
            var c = cache[t];
            c.canvasGroup.DOKill();

            // 2. Reset trạng thái ban đầu (Alpha = 0, Scale = 0)
            ResetForAppear(t);

            // 3. Tạo Sequence để quản lý đồng bộ
            Sequence seq = DOTween.Sequence();
            seq.SetDelay(currentDelay);

            // Di chuyển/Xoay/Scale
            switch (group.animType)
            {
                case AppearGroupAnimType.Scale:
                    seq.Append(t.DOScale(c.scale, group.duration).SetEase(group.ease));
                    break;
                case AppearGroupAnimType.MoveLeft:
                    SetupMove(seq, t, Vector2.left, group);
                    break;
                case AppearGroupAnimType.MoveRight:
                    SetupMove(seq, t, Vector2.right, group);
                    break;
                case AppearGroupAnimType.MoveUp:
                    SetupMove(seq, t, Vector2.up, group);
                    break;
                case AppearGroupAnimType.MoveDown:
                    SetupMove(seq, t, Vector2.down, group);
                    break;
                case AppearGroupAnimType.Rotate:
                    t.localEulerAngles = c.localRot + group.rotateAmount;

                    seq.Append(
                        t.DORotate(c.localRot, group.duration)
                         .SetEase(group.ease)
                    );

                    seq.Join(
                        t.DOScale(c.scale, group.duration)
                         .SetEase(group.ease)
                    );
                    break;

            }

            // Luôn đảm bảo Alpha chạy song song
            seq.Join(
            DOTween.To(() => c.canvasGroup.alpha, x => c.canvasGroup.alpha = x, 1f, group.duration)
        .SetEase(group.ease));


            // Bật lại tương tác khi hoàn thành Sequence
            seq.OnComplete(() => {
                c.canvasGroup.interactable = true;
                c.canvasGroup.blocksRaycasts = true;
            });

            currentDelay += group.delayBetween;
        }
    }

    void SetupMove(Sequence seq, Transform t, Vector2 dir, AppearAnimGroup g)
    {
        var c = cache[t];

        if (c.isRect)
        {
            RectTransform rect = (RectTransform)t;
            rect.anchoredPosition = c.anchoredPos + dir * g.moveDistance;

            seq.Append(
                rect.transform.DOLocalMove(c.anchoredPos, g.duration)
                    .SetEase(g.ease)
            );
        }
        else
        {
            t.localPosition = c.localPos + (Vector3)(dir * g.moveDistance);

            seq.Append(
                t.DOLocalMove(c.localPos, g.duration)
                    .SetEase(g.ease)
            );
        }

        seq.Join(
            t.DOScale(c.scale, g.duration)
             .SetEase(g.ease)
        );
    }


    void ResetForAppear(Transform t)
    {
        var c = cache[t];

        // Kill toàn bộ tween
        t.DOKill(true);
        c.canvasGroup.DOKill(true);

        // RESET POSITION
        if (c.isRect)
            ((RectTransform)t).anchoredPosition = c.anchoredPos;
        else
            t.localPosition = c.localPos;

        // RESET ROTATION
        t.localEulerAngles = c.localRot;

        // RESET SCALE & ALPHA
        t.localScale = Vector3.zero;
        c.canvasGroup.alpha = 0f;
        c.canvasGroup.interactable = false;
        c.canvasGroup.blocksRaycasts = false;
    }


    void Cache(Transform t)
    {
        if (cache.ContainsKey(t)) return;
        CacheData c = new() {
            scale = t.localScale,
            localRot = t.localEulerAngles,
            isRect = t is RectTransform
        };
        if (c.isRect) c.anchoredPos = ((RectTransform)t).anchoredPosition;
        else c.localPos = t.localPosition;

        c.canvasGroup = t.GetComponent<CanvasGroup>() ?? t.gameObject.AddComponent<CanvasGroup>();
        cache.Add(t, c);
    }
}

[System.Serializable]
public class AppearAnimGroup
{
    public string groupName;
    public AppearGroupAnimType animType;
    public List<Transform> targets = new();

    [Header("Settings")]
    public float duration = 0.25f;
    public float delayBetween = 0.05f;
    public Ease ease = Ease.OutBack;

    [Header("Move")]
    public float moveDistance = 200f;

    [Header("Rotate")]
    public Vector3 rotateAmount = new(0, 0, 90f);
}