Shader "UI/CardGlowSDF"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _RectSize ("Glow Rect Size (px)", Vector) = (360, 480, 0, 0)
        _CardSize ("Card Size (px)", Vector) = (300, 420, 0, 0)
        _Radius ("Corner Radius", Float) = 16
        _Width ("Glow Width", Float) = 25
        _Power ("Falloff Power", Float) = 2
        _Intensity ("Intensity", Float) = 1.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }
        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _RectSize;
            float4 _CardSize;
            float _Radius, _Width, _Power, _Intensity;

            struct appdata { float4 v : POSITION; float4 c : COLOR; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; fixed4 c : COLOR; float2 uv : TEXCOORD0; };

            v2f vert(appdata i)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(i.v);
                o.c = i.c;
                o.uv = i.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Vị trí pixel so với tâm lá bài
                float2 p = (i.uv - 0.5) * _RectSize.xy;

                // Khoảng cách tới hình chữ nhật bo góc (d > 0 là bên ngoài lá bài)
                float2 q = abs(p) - (_CardSize.xy * 0.5 - _Radius);
                float d = length(max(q, 0)) + min(max(q.x, q.y), 0) - _Radius;

                // Sáng nhất sát viền, mờ dần ra ngoài
                float g = pow(saturate(1 - d / _Width), _Power);

                return fixed4(i.c.rgb * _Intensity, i.c.a * g);
            }
            ENDCG
        }
    }
}