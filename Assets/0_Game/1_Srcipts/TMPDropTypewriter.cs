using UnityEngine;
using TMPro;
using DG.Tweening;

public class TMPDropTypewriter : MonoBehaviour
{
    public bool enableSound = false;
    [Header("Drop In")]
    public float charDelay = 0.05f;
    public float dropDistance = 40f;
    public float dropDuration = 0.35f;
    public Ease dropEase = Ease.OutBack;

    [Header("Idle Wave")]
    public float waveAmplitude = 6f;
    public float waveSpeed = 2f;
    public float floatAmplitude = 4f;

    [Header("Idle Scale")]
    public float scaleAmplitude = 0.15f;
    public float scaleSpeed = 1.5f;

    [Header("Idle Blend")]
    public float idleBlendTime = 0.6f; 

    private float idleStartTime;

    [Header("Sound")]
    public string dropSoundKey = "text_drop";
    public bool randomPitch = true;
    public float pitchMin = 0.76f;
    public float pitchMax = 1.3f;

    private TMP_Text tmp;
    private CanvasGroup canvasGroup;
    private TMP_TextInfo textInfo;
    private Vector3[][] baseVertices;
    private Tweener idleTween; 

    void Awake()
    {
        tmp = GetComponent<TMP_Text>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void Start()
    {
        PlayIfNeeded();
    }

    public void PlayIfNeeded()
    {
        if (!gameObject.activeInHierarchy) return;
        Play();
    }

    public void Play()
    {
        DOTween.Kill(this);
        if (idleTween != null) idleTween.Kill();

        tmp.maxVisibleCharacters = 999;
        tmp.ForceMeshUpdate();
        textInfo = tmp.textInfo;

        baseVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            baseVertices[i] = textInfo.meshInfo[i].vertices.Clone() as Vector3[];
        }

        canvasGroup.alpha = 0;
        tmp.maxVisibleCharacters = 0;
        canvasGroup.DOFade(1f, 0.2f).SetTarget(this);

        int visibleCount = 0;
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int charIndex = i;
            visibleCount++;

            DOVirtual.DelayedCall(visibleCount * charDelay, () =>
            {
                tmp.maxVisibleCharacters = charIndex + 1;
                DoDrop(charIndex);

                if (enableSound)
                    PlayCharSound();

            }).SetTarget(this);
        }

        float totalDropTime = (visibleCount * charDelay) + dropDuration;

        DOVirtual.DelayedCall(totalDropTime, () =>
        {
            StartIdle();
            enableSound = false;
        }).SetTarget(this);
    }

    void PlayCharSound()
    {
        if (string.IsNullOrEmpty(dropSoundKey)) return;

        float pitch = randomPitch ? Random.Range(pitchMin, pitchMax) : 1f;
    }

    void DoDrop(int charIndex)
    {
        var charInfo = textInfo.characterInfo[charIndex];
        int matIndex = charInfo.materialReferenceIndex;
        int vertIndex = charInfo.vertexIndex;
        Vector3[] verts = textInfo.meshInfo[matIndex].vertices;

        DOTween.To(() => dropDistance, y =>
        {
            if (textInfo.characterCount <= charIndex) return;

            for (int j = 0; j < 4; j++)
            {
                verts[vertIndex + j] = baseVertices[matIndex][vertIndex + j] + Vector3.up * y;
            }
            tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
        }, 0, dropDuration).SetEase(dropEase).SetTarget(this);
    }

    void StartIdle()
    {
        idleStartTime = Time.time;

        float timer = 0;
        idleTween = DOTween.To(() => timer, x => timer = x, 1f, 1f)
            .OnUpdate(() => AnimateIdle(Time.time))
            .SetLoops(-1)
            .SetTarget(this);
    }

    void AnimateIdle(float time)
    {
        float t = time - idleStartTime;
        float blend = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / idleBlendTime));

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;
            Vector3[] verts = textInfo.meshInfo[matIndex].vertices;

            float phase = t * waveSpeed + i * 0.35f;
            Vector3 offset =
                (Vector3.up * Mathf.Sin(phase) * floatAmplitude +
                 Vector3.right * Mathf.Cos(phase * 0.7f) * waveAmplitude) * blend;

            float scalePhase = t * scaleSpeed + i * 0.8f;
            float scale = 1f + Mathf.Sin(scalePhase) * scaleAmplitude * blend;

            Vector3 center = Vector3.zero;
            for (int j = 0; j < 4; j++)
                center += baseVertices[matIndex][vertIndex + j];
            center /= 4f;

            for (int j = 0; j < 4; j++)
            {
                Vector3 basePos = baseVertices[matIndex][vertIndex + j];
                verts[vertIndex + j] = center + (basePos - center) * scale + offset;
            }
        }

        tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    private void OnDisable()
    {
        DOTween.Kill(this);
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }

}