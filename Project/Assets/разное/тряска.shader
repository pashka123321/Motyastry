Shader "Custom/ShakeEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShakeIntensity ("Shake Intensity", Range(0,1)) = 0.2
        _ShakeSpeed ("Shake Speed", Range(0,50)) = 10.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _ShakeIntensity;
            float _ShakeSpeed;

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

            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
            }

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float timeFactor = floor(_Time.y * _ShakeSpeed);
                float2 randomShake = float2(rand(float2(timeFactor, 1.0)), rand(float2(1.0, timeFactor)));
                randomShake = (randomShake - 0.5) * _ShakeIntensity;

                float2 uv = i.uv + randomShake;
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
