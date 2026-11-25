Shader "UI/RectOutlineGlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _OutlineColor ("Outline Color", Color) = (0.25, 0.9, 1.0, 1)
        _OutlineWidth ("Outline Width (UV)", Range(0.001, 0.2)) = 0.025
        _GlowWidth ("Glow Width (UV)", Range(0.001, 0.3)) = 0.08
        _GlowIntensity ("Glow Intensity", Range(0, 4)) = 1.2
        _GlowOpacity ("Glow Opacity", Range(0, 1)) = 0.5

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex        : SV_POSITION;
                fixed4 color         : COLOR;
                float2 uv            : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _GlowWidth;
            float _GlowIntensity;
            float _GlowOpacity;
            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv) * i.color;

                // Khoảng cách UV tới cạnh gần nhất của hình chữ nhật.
                float edge = min(
                    min(i.uv.x, 1.0 - i.uv.x),
                    min(i.uv.y, 1.0 - i.uv.y)
                );

                // Viền sắc ở sát cạnh và glow mờ dần ra ngoài.
                float outline = 1.0 - smoothstep(
                    _OutlineWidth * 0.65,
                    _OutlineWidth,
                    edge
                );

                float glow = 1.0 - smoothstep(
                    _OutlineWidth,
                    _GlowWidth,
                    edge
                );

                // Chỉ thêm hiệu ứng quanh vùng sát mép.
                float effect = saturate(max(outline, glow));
                fixed3 glowRGB = _OutlineColor.rgb *
                                 glow * _GlowOpacity * _GlowIntensity;

                fixed4 result;
                result.rgb = tex.rgb + glowRGB;
                result.a = max(tex.a, glow * _GlowOpacity);

                result.rgb = lerp(
                    result.rgb,
                    _OutlineColor.rgb,
                    outline * _OutlineColor.a
                );
                result.a = max(result.a, outline * _OutlineColor.a);

                #ifdef UNITY_UI_CLIP_RECT
                result.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(result.a - 0.001);
                #endif

                return result;
            }
            ENDCG
        }
    }
}