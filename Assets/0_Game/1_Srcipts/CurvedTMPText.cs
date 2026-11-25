
using TMPro;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class CurvedTMPText : MonoBehaviour
{
    [Header("Curve Settings")]
    [Range(-1000f, 1000f)]
    public float curveAmount = 100f;

    [Header("Direction")]
    public bool reverseCurve = false;

    private TMP_Text tmpText;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        tmpText = GetComponent<TMP_Text>();
        UpdateCurve();
    }

    private void LateUpdate()
    {
        UpdateCurve();
    }

    private void UpdateCurve()
    {
        if (tmpText == null) return;

        tmpText.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpText.textInfo;

        float width = tmpText.rectTransform.rect.width;
        if (width <= 0) return;

        float direction = reverseCurve ? -1f : 1f;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] vertices =
                textInfo.meshInfo[materialIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                Vector3 vertex = vertices[vertexIndex + j];

                float x = vertex.x -
                    tmpText.rectTransform.rect.center.x;

                float normalizedX = x / width;

                float curve = normalizedX * normalizedX
                    * curveAmount * direction;

                vertex.y += curve;

                vertices[vertexIndex + j] = vertex;
            }
        }

        tmpText.UpdateVertexData(
            TMP_VertexDataUpdateFlags.Vertices
        );
    }
}
