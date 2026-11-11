Shader "Custom/CRTEffect"
{
    Properties
    {
        _ScanlineCount     ("Scanline Count", Float) = 240
        _ScanlineIntensity ("Scanline Intensity", Range(0,1)) = 0.35
        _Curvature         ("Screen Curvature", Range(0,0.3)) = 0.05
        _ChromaOffset      ("Chromatic Offset (px)", Range(0,5)) = 1.5
        _VignetteIntensity ("Vignette Intensity", Range(0,1)) = 0.35
        _NoiseIntensity    ("Noise Intensity", Range(0,0.2)) = 0.04
        _FlickerIntensity  ("Flicker Intensity", Range(0,0.1)) = 0.02
        _Brightness        ("Brightness", Range(0.5,2)) = 1.15
        _Blur              ("Horizontal Blur (px)", Range(0,2)) = 0.6
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off ZTest Always Cull Off

        Pass
        {
            Name "CRT"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _ScanlineCount;
            float _ScanlineIntensity;
            float _Curvature;
            float _ChromaOffset;
            float _VignetteIntensity;
            float _NoiseIntensity;
            float _FlickerIntensity;
            float _Brightness;
            float _Blur;

            float Hash(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float2 CurveUV(float2 uv)
            {
                uv = uv * 2.0 - 1.0;
                float2 offset = abs(uv.yx) * _Curvature;
                uv += uv * offset * offset * 4.0;
                return uv * 0.5 + 0.5;
            }

            float3 SampleScene(float2 uv)
            {
                return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).rgb;
            }

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv = CurveUV(input.texcoord);

                // Ngoài vùng màn hình cong -> đen
                if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
                    return float4(0, 0, 0, 1);

                float2 px = float2(1.0 / _ScreenParams.x, 0);

                // Chromatic aberration: R lệch phải, B lệch trái
                float3 col;
                col.r = SampleScene(uv + px * _ChromaOffset).r;
                col.g = SampleScene(uv).g;
                col.b = SampleScene(uv - px * _ChromaOffset).b;

                // Blur ngang nhẹ cho cảm giác tín hiệu analog
                float3 blur = SampleScene(uv + px * _Blur) + SampleScene(uv - px * _Blur);
                col = lerp(col, blur * 0.5, 0.35);

                // Scanlines
                float scan = sin(uv.y * _ScanlineCount * 6.28318) * 0.5 + 0.5;
                col *= lerp(1.0, scan, _ScanlineIntensity);

                // Noise
                float n = Hash(uv * _ScreenParams.xy + _Time.y);
                col += (n - 0.5) * _NoiseIntensity;

                // Flicker
                col *= 1.0 + sin(_Time.y * 60.0) * _FlickerIntensity;

                // Vignette
                float2 v = uv * (1.0 - uv.yx);
                float vig = pow(saturate(v.x * v.y * 15.0), _VignetteIntensity);
                col *= vig;

                col *= _Brightness;
                return float4(saturate(col), 1);
            }
            ENDHLSL
        }
    }
}