Shader "UI/GlowOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _GlowColor ("Glow Color", Color) = (1.0, 0.75, 0.2, 1)
        _GlowWidth ("Glow Width (px)", Range(0, 20)) = 3
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 2
        _GlowSoftness ("Glow Softness", Range(0, 1)) = 0.5

        _PulseSpeed ("Pulse Speed", Range(0, 5)) = 1.5
        _PulseAmount ("Pulse Amount", Range(0, 1)) = 0.25

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

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
        }

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
            float4 _MainTex_TexelSize;   // (1/width, 1/height, width, height)

            float4 _Color;
            float4 _GlowColor;
            float _GlowWidth;
            float _GlowIntensity;
            float _GlowSoftness;
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

            // Tính alpha "phóng to" bằng cách sample 8 hướng xung quanh
            float GetDilatedAlpha(float2 uv, float radius)
            {
                float2 texel = _MainTex_TexelSize.xy;
                float a = 0;

                // 8 hướng
                a = max(a, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( radius, 0) * texel).a);
                a = max(a, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-radius, 0) * texel).a);
                a = max(a, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0,  radius) * texel).a);
a = max(a, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0, -radius) * texel).a);                a = max(a, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( radius,  radius) * texel).a);
                a = max(a, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-radius,  radius) * texel).a);
                a = max(a, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( radius, -radius) * texel).a);
                a = max(a, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-radius, -radius) * texel).a);

                return a;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                tex *= IN.color * _Color;

                // Pulse: dao động 0 -> 1 theo thời gian
                float pulse = 1.0 + sin(_Time.y * _PulseSpeed * 6.2831) * _PulseAmount;

                // 2 lớp glow (như box-shadow nhiều lớp trong CSS)
                float innerA = GetDilatedAlpha(IN.uv, _GlowWidth);
                float outerA = GetDilatedAlpha(IN.uv, _GlowWidth * (1.0 + _GlowSoftness * 2.0));

                // Chỉ glow ở vùng NGOÀI sprite gốc
                float outlineMask = saturate(innerA - tex.a);

                float4 glow =
                    _GlowColor * outlineMask * _GlowIntensity * pulse * 1.0 +
                    _GlowColor * outerA * _GlowIntensity * pulse * 0.4 * (1 - innerA);

                half4 col = tex;
                col.rgb = lerp(glow.rgb, tex.rgb, tex.a);
                col.a = max(tex.a, outlineMask);

                return col;
            }
            ENDHLSL
        }
    }
}