Shader "Custom/BlurShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Size ("Blur Size", Float) = 1.0
        _DepthTex ("Depth Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _DepthTex;
            float _Size;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float depth = tex2D(_DepthTex, i.uv).r;
                // Настроить порог глубины, чтобы определить, что UI на определенной глубине
                if (depth > 0.9) // Значение порога может потребовать настройки
                {
                    return tex2D(_MainTex, i.uv); // Не применять размытие к UI
                }

                float2 texelSize = 1.0 / float2(512.0, 512.0); // Замените 512.0 на реальное разрешение текстуры
                half4 color = half4(0,0,0,0);
                int blurSize = 5;

                for (int x = -blurSize; x <= blurSize; x++)
                {
                    for (int y = -blurSize; y <= blurSize; y++)
                    {
                        float2 offset = float2(x, y) * _Size * texelSize;
                        color += tex2D(_MainTex, i.uv + offset);
                    }
                }

                return color / pow((blurSize * 2 + 1), 2);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
