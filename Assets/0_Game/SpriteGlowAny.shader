Shader "UI/SpriteGlowAny"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        [HDR] _Color ("Glow Color", Color) = (1, 0.8, 0.2, 1)
        _Width ("Glow Width (texture px)", Float) = 12
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

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            float _Width, _Power, _Intensity;

            struct appdata { float4 v : POSITION; float4 c : COLOR; float2 uv : TEXCOORD0; float4 rect : TEXCOORD1; };
            struct v2f { float4 pos : SV_POSITION; fixed4 c : COLOR; float2 uv : TEXCOORD0; float4 rect : TEXCOORD1; };

            v2f vert(appdata i)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(i.v);
                o.c = i.c * _Color;
                o.uv = i.uv;
                o.rect = i.rect;
                return o;
            }

            // Alpha của sprite, ngoài UV rect của sprite = 0 (không đọc nhầm sprite khác trong atlas)
            float Alpha(float2 uv, float4 r)
            {
                float2 m = step(r.xy, uv) * step(uv, r.zw);
                return tex2D(_MainTex, uv).a * m.x * m.y;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 stepUV = _MainTex_TexelSize.xy * _Width / 3.0;

                float sum = 0, wsum = 0;
                [unroll] for (int ring = 1; ring <= 3; ring++)
                {
                    float w = pow(1 - (ring - 1) / 3.0, _Power); // vòng càng xa càng yếu
                    [unroll] for (int d = 0; d < 8; d++)
                    {
                        float ang = d * 0.7853982; // 2PI/8
                        float2 offs = float2(cos(ang), sin(ang)) * stepUV * ring;
                        sum += Alpha(i.uv + offs, i.rect) * w;
                        wsum += w;
                    }
                }

                float g = saturate(sum / wsum * _Intensity * 2);
                g *= 1 - Alpha(i.uv, i.rect); // chỉ glow bên ngoài sprite

                return fixed4(i.c.rgb, i.c.a * g);
            }
            ENDCG
        }
    }
}