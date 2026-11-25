Shader "UI/CardGlow"
{
    Properties
    {
        _GlowColor ("Glow Color (HDR)", Color) = (2.2, 0.5, 1.4, 1)
        _GlowRange ("Glow Range (px)", Range(1, 80)) = 24
        _GlowSharpness ("Glow Sharpness", Range(0.02, 1)) = 0.18
        _InnerMargin ("Inner Margin (px)", Range(0, 60)) = 20
        _CornerRadius ("Corner Radius (px)", Range(0, 60)) = 8

        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1.5
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.2

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        Stencil { Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

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

            float4 _GlowColor;
            float _GlowRange;
            float _GlowSharpness;
            float _InnerMargin;
            float _CornerRadius;
            float _PulseSpeed;
            float _PulseAmount;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Pixel space: đều glow theo mọi hướng, không méo theo tỉ lệ quad
                float2 px = IN.uv * _MainTex_TexelSize.zw;

                // Rect của BUTTON bên trong quad glow (cách đều _InnerMargin px)
                float2 center = _MainTex_TexelSize.zw * 0.5;
                float2 halfSize = center - _InnerMargin;

                // Signed distance tới rect bo góc
                float2 d = abs(px - center) - halfSize + _CornerRadius;
                float dist = length(max(d, 0.0)) + min(max(d.x, d.y), 0.0) - _CornerRadius;

                float pulse = 1.0 + sin(_Time.y * _PulseSpeed * 6.2831) * _PulseAmount;

                // Exponential falloff: mạnh nhất NGAY SÁT viền, mờ dần ra ngoài vô hạn
                // Bên trong button cũng thấm nhẹ một chút cho giống card STS
                float glow = exp(-max(dist, 0.0) * _GlowSharpness);
                glow += 0.25 * exp(dist * 0.15) * (dist < 0.0);  // thấm nhẹ vào trong
                glow *= _GlowColor.a * pulse;

                glow = saturate(glow);
                return half4(_GlowColor.rgb * glow * IN.color.rgb, glow);
            }
            ENDHLSL
        }
    }
}