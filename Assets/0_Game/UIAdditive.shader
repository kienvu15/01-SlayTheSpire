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

        // --- Stencil cho UI Mask ---
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True" }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha One
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            float4 _RectSize;
            float4 _CardSize;
            float _Radius, _Width, _Power, _Intensity;
            float4 _ClipRect;

            struct appdata { float4 v : POSITION; float4 c : COLOR; float2 uv : TEXCOORD0; };
            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed4 c : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
            };

            v2f vert(appdata i)
            {
                v2f o;
                o.worldPos = i.v;
                o.pos = UnityObjectToClipPos(i.v);
                o.c = i.c;
                o.uv = i.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 p = (i.uv - 0.5) * _RectSize.xy;

                float2 q = abs(p) - (_CardSize.xy * 0.5 - _Radius);
                float d = length(max(q, 0)) + min(max(q.x, q.y), 0) - _Radius;

                float g = pow(saturate(1 - d / _Width), _Power);

                fixed4 col = fixed4(i.c.rgb * _Intensity, i.c.a * g);

                #ifdef UNITY_UI_CLIP_RECT
                col.a *= UnityGet2DClipping(i.worldPos.xy, _ClipRect);
                #endif

                return col;
            }
            ENDCG
        }
    }
}