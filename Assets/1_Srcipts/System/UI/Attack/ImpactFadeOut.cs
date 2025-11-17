using UnityEngine;
using System.Collections;
using System;

public class ImpactFadeOut : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    private Action<GameObject> onFinished;
    private SpriteRenderer sr;
    private CanvasGroup cg;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        cg = GetComponent<CanvasGroup>();
    }

    public void PlayFadeOut(Action<GameObject> callback)
    {
        onFinished = callback;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        float t = 0;

        // Lấy alpha ban đầu
        float startA = 1f;
        float a = 1f;

        if (sr != null)
            startA = sr.color.a;
        else if (cg != null)
            startA = cg.alpha;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            a = Mathf.Lerp(startA, 0, t / fadeDuration);

            if (sr != null)
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, a);
            else if (cg != null)
                cg.alpha = a;

            yield return null;
        }

        // reset alpha khi return pool
        if (sr != null)
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1);
        else if (cg != null)
            cg.alpha = 1;

        onFinished?.Invoke(gameObject);
    }
}
