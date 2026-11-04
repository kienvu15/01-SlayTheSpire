
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIBezierConnection : MaskableGraphic
{
    [Header("Line Settings")]
    public float thickness = 4f;
    public int segments = 20;

    private Vector2 startPoint;
    private Vector2 endPoint;
    private float curveAmount;

    public void SetPoints(Vector2 start, Vector2 end, float curve)
    {
        startPoint = start;
        endPoint = end;
        curveAmount = curve;

        SetVerticesDirty();
    }

    protected override void Awake()
    {
        base.Awake();
        raycastTarget = false;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 direction = (endPoint - startPoint).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x);

        Vector2 mid = (startPoint + endPoint) * 0.5f;
        mid += perpendicular * curveAmount;

        Vector2 previous = startPoint;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;

            Vector2 current =
                Mathf.Pow(1 - t, 2) * startPoint +
                2 * (1 - t) * t * mid +
                Mathf.Pow(t, 2) * endPoint;

            Vector2 dir = (current - previous).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x) * thickness * 0.5f;

            UIVertex[] quad = new UIVertex[4];

            quad[0] = CreateVertex(previous - normal);
            quad[1] = CreateVertex(previous + normal);
            quad[2] = CreateVertex(current + normal);
            quad[3] = CreateVertex(current - normal);

            vh.AddUIVertexQuad(quad);

            previous = current;
        }
    }

    private UIVertex CreateVertex(Vector2 position)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;
        vertex.position = position;
        return vertex;
    }
}
