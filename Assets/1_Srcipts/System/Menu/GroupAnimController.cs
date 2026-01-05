using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum GroupAnimType
{
    ScaleDisappear,
    MoveLeftDisappear,
    MoveRightDisappear,
    MoveUpDisappear,
    MoveDownDisappear,
    RotateDisappear
}

public class GroupAnimManager : MonoBehaviour
{
    [Header("Animation Groups")]
    public List<AnimGroup> groups = new List<AnimGroup>();

    public bool disableAfter = true;

    //Click
    public IEnumerator PlayModeDisappear()
    {
        PlayGroupByName("PlayMode");
        yield return new WaitForSeconds(0.1f);
        PlayGroupByName("BackButton");
        yield return new WaitForSeconds(0.1f);
        PlayGroupByName("SkinSelect");
    }

    public void MenuModeDisappear()
    {
        PlayGroupByName("MenuButton");
        PlayGroupByName("Logo");
        PlayGroupByName("PlayerData");
    }

    //BUTTON

    public void PlayAllGroups()
    {
        for(int i = 0; i < groups.Count; i++)
        {
            Play(groups[i]);
        }
    }

    public void PlayGroup(int groupIndex)
    {
        if (groupIndex < 0 || groupIndex >= groups.Count) return;
        Play(groups[groupIndex]);
    }

    public void PlayGroupByName(string groupName)
    {
        AnimGroup group = groups.Find(g => g.groupName == groupName);
        if (group != null)
            Play(group);
    }

    // =========================

    void Play(AnimGroup group)
    {
        float delay = 0f;

        foreach (Transform t in group.targets)
        {
            if (t == null) continue;

            t.DOKill(true);

            switch (group.animType)
            {
                case GroupAnimType.ScaleDisappear:
                    PlayScale(t, group, delay);
                    break;

                case GroupAnimType.MoveLeftDisappear:
                    PlayMove(t, Vector2.left, group, delay);
                    break;

                case GroupAnimType.MoveRightDisappear:
                    PlayMove(t, Vector2.right, group, delay);
                    break;

                case GroupAnimType.MoveUpDisappear:
                    PlayMove(t, Vector2.up, group, delay);
                    break;

                case GroupAnimType.MoveDownDisappear:
                    PlayMove(t, Vector2.down, group, delay);
                    break;

                case GroupAnimType.RotateDisappear:
                    PlayRotate(t, group, delay);
                    break;
            }

            delay += group.delayBetween;
        }
    }

    // =========================
    // ANIM METHODS
    // =========================

    void PlayScale(Transform t, AnimGroup g, float delay)
    {
        t.DOScale(Vector3.zero, g.duration)
         .SetEase(g.ease)
         .SetDelay(delay)
         .OnComplete(() => Disable(t));
    }

    void PlayMove(Transform t, Vector2 dir, AnimGroup g, float delay)
    {
        RectTransform rect = t as RectTransform;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);

        if (rect != null)
        {
            Vector2 target = rect.anchoredPosition + dir * g.moveDistance;
            seq.Append(rect.transform.DOLocalMove(target, g.duration).SetEase(g.ease));
        }
        else
        {
            Vector3 target = t.position + (Vector3)(dir * g.moveDistance);
            seq.Append(t.DOMove(target, g.duration).SetEase(g.ease));
        }

        seq.Join(t.DOScale(Vector3.zero, g.duration * 0.9f));
        seq.OnComplete(() => Disable(t));
    }

    void PlayRotate(Transform t, AnimGroup g, float delay)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);

        seq.Append(
            t.DORotate(g.rotateAmount, g.duration, RotateMode.LocalAxisAdd)
             .SetEase(g.ease)
        );

        seq.Join(t.DOScale(Vector3.zero, g.duration));
        seq.OnComplete(() => Disable(t));
    }

    void Disable(Transform t)
    {
        if (disableAfter)
            t.gameObject.SetActive(false);
    }
}

[System.Serializable]
public class AnimGroup
{
    public string groupName;

    public GroupAnimType animType;

    public List<Transform> targets = new List<Transform>();

    [Header("Settings")]
    public float duration = 0.25f;
    public float delayBetween = 0.05f;
    public Ease ease = Ease.InBack;

    [Header("Move")]
    public float moveDistance = 200f;

    [Header("Rotate")]
    public Vector3 rotateAmount = new Vector3(0, 0, 90f);
}
