Shader "Custom/WavingGrassShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Float) = 1.0
        _WaveAmplitude ("Wave Amplitude", Float) = 0.05
        _Frequency ("Wave Frequency", Float) = 3.0
    }
    
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _WaveSpeed;
            float _WaveAmplitude;
            float _Frequency;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;

                // Убедимся, что искажение не изменяет UV-координаты
                float wave = sin(v.vertex.x * _Frequency + _Time.y * _WaveSpeed) * _WaveAmplitude;
                v.vertex.y += wave; // Искажение только по оси Y для эффекта колебания
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // Оставляем UV координаты нетронутыми
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Прямое текстурирование без изменений UV
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }

    FallBack "Diffuse"
}
