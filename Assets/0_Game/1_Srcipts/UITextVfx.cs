
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UITextVfx : MonoBehaviour
{
    public TextMeshProUGUI text;

    [Header("Char Pop")]
    public float popScale = 1.3f;
    public float popDuration = 0.25f;
    public float charDelay = 0.05f;

    [Header("Global")]
    public float floatUp = 0.7f;
    public float lifeTime = 1.6f;

    private TMP_TextInfo textInfo;
    private Vector3[][] originalVertices;
    private float[] charScales;

    void Start()
    {
        Init("Choose your debt.");
    }

    public void Init(string message)
    {
        text.text = message;
        text.alpha = 1f;

        text.ForceMeshUpdate();
        textInfo = text.textInfo;

        originalVertices = new Vector3[textInfo.meshInfo.Length][];

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalVertices[i] =
                (Vector3[])textInfo.meshInfo[i].vertices.Clone();
        }

        charScales = new float[textInfo.characterCount];

        for (int i = 0; i < charScales.Length; i++)
            charScales[i] = 0f;

        PlayCharPop();
        PlayFloatUp();
    }

    void PlayCharPop()
    {
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            int index = i;

            DOTween.To(
                () => 0f,
                v => charScales[index] = v,
                popScale,
                popDuration
            )
            .SetDelay(index * charDelay)
            .SetEase(Ease.OutBack);
        }
    }

    void PlayFloatUp()
    {
        transform.DOMoveY(
            transform.position.y + floatUp,
            lifeTime
        ).SetEase(Ease.OutSine);

        text.DOFade(0f, lifeTime)
            .SetDelay(0.3f)
            .OnComplete(() => Destroy(gameObject));
    }

    void LateUpdate()
    {
        if (originalVertices == null || textInfo == null)
            return;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            int matIndex = charInfo.materialReferenceIndex;
            int vertIndex = charInfo.vertexIndex;

            Vector3[] srcVerts = originalVertices[matIndex];
            Vector3[] dstVerts = textInfo.meshInfo[matIndex].vertices;

            Vector3 center =
                (srcVerts[vertIndex] + srcVerts[vertIndex + 2]) * 0.5f;

            float scale = charScales[i];

            for (int j = 0; j < 4; j++)
            {
                Vector3 dir = srcVerts[vertIndex + j] - center;

                dstVerts[vertIndex + j] =
                    center + dir * scale;
            }
        }

        // Update toàn bộ vertex data của TextMeshProUGUI
        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    private void OnDestroy()
    {
        DOTween.Kill(transform);
        DOTween.Kill(text);
    }
}
