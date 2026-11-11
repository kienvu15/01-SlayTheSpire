using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

[RequireComponent(typeof(Image))]
public class SpriteGlowMesh : BaseMeshEffect
{
    private static readonly int WidthId = Shader.PropertyToID("_Width");

    protected override void OnEnable()
    {
        base.OnEnable();
        // Cần kênh TexCoord1 để truyền UV rect vào shader
        if (graphic != null && graphic.canvas != null)
            graphic.canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        var img = graphic as Image;
        if (!IsActive() || img == null || img.sprite == null) return;

        Sprite sp = img.sprite;
        Material mat = img.material;
        float width = (mat != null && mat.HasProperty(WidthId)) ? mat.GetFloat(WidthId) : 0f; // đơn vị: pixel của texture

        Rect r = img.GetPixelAdjustedRect();
        Vector4 o = DataUtility.GetOuterUV(sp); // minU, minV, maxU, maxV

        // Độ nới theo đơn vị UI và theo UV
        float px = width * r.width / sp.rect.width;
        float py = width * r.height / sp.rect.height;
        float du = (o.z - o.x) * width / sp.rect.width;
        float dv = (o.w - o.y) * width / sp.rect.height;

        Color32 c = img.color;
        var rectUV = new Vector4(o.x, o.y, o.z, o.w);

        vh.Clear();
        AddV(vh, r.xMin - px, r.yMin - py, o.x - du, o.y - dv, c, rectUV);
        AddV(vh, r.xMin - px, r.yMax + py, o.x - du, o.w + dv, c, rectUV);
        AddV(vh, r.xMax + px, r.yMax + py, o.z + du, o.w + dv, c, rectUV);
        AddV(vh, r.xMax + px, r.yMin - py, o.z + du, o.y - dv, c, rectUV);
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    private static void AddV(VertexHelper vh, float x, float y, float u, float v, Color32 c, Vector4 rect)
    {
        var vert = UIVertex.simpleVert;
        vert.position = new Vector3(x, y, 0);
        vert.color = c;
        vert.uv0 = new Vector4(u, v, 0, 0);
        vert.uv1 = rect;
        vh.AddVert(vert);
    }

#if UNITY_EDITOR
    // Đổi _Width trên material thì bấm Refresh hoặc đổi gì đó trên Image để rebuild
    protected override void OnValidate()
    {
        base.OnValidate();
        if (graphic != null) graphic.SetVerticesDirty();
    }
#endif
}