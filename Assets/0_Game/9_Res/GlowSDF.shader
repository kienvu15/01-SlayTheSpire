Shader "UI/GlowSDF"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _GlowColor ("Glow Color (HDR)", Color) = (2.5, 1.6, 0.4, 1)
        _GlowWidth ("Glow Width", Range(0, 0.25)) = 0.06
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 2
        _GlowPower ("Glow Falloff", Range(0.5, 8)) = 2.5

        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1.5
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.25

        _CornerRadius ("Corner Radius", Range(0, 0.5)) = 0.02

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _MainTex_TexelSize;

            float4 _Color;
            float4 _GlowColor;
            float _GlowWidth;
            float _GlowIntensity;
            float _GlowPower;
            float _PulseSpeed;
            float _PulseAmount;
            float _CornerRadius;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                return OUT;
            }

            // Khoảng cách có dấu tới rect [0,1]^2 với bo góc
            float RectSDF(float2 uv, float2 halfSize, float radius)
            {
                float2 d = abs(uv - 0.5) - halfSize + radius;
                return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0) - radius;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float dist = RectSDF(IN.uv, 0.5, _CornerRadius);

                // Thân button: alpha khử răng cưa bằng fwidth
                float aa = fwidth(dist);
                float bodyAlpha = 1.0 - smoothstep(-aa, aa, dist);

                // Glow: suy giảm theo khoảng cách ra khỏi viền
                float pulse = 1.0 + sin(_Time.y * _PulseSpeed * 6.2831) * _PulseAmount;
                float glow = pow(saturate(1.0 - dist / _GlowWidth), _GlowPower)
                             * (1.0 - bodyAlpha)              // không glow đè lên thân
                             * _GlowIntensity * pulse;

                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half4 body = tex * IN.color * _Color;
                body.a *= bodyAlpha;

                float3 rgb = body.rgb * body.a + _GlowColor.rgb * glow;
                float a = saturate(body.a + glow);

                return half4(rgb, a);
            }
            ENDHLSL
        }
    }
}