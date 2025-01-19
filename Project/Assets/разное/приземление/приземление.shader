Shader "Custom/FireDistortionShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _DistortionTex ("Distortion Texture", 2D) = "white" {}
        _DistortionStrength ("Distortion Strength", Range(0, 1)) = 0.5
        _TimeMultiplier ("Time Multiplier", Float) = 1.0
        _FireDirection ("Fire Direction (degrees)", Range(0, 360)) = 0 // Направление огня
        _Color1 ("Fire Color 1", Color) = (1, 0.2, 0.2, 1) // Тусклый красный
        _Color2 ("Fire Color 2", Color) = (1, 0.4, 0.2, 1) // Тусклый оранжевый
        _Color3 ("Fire Color 3", Color) = (1, 0.6, 0.2, 1) // Тусклый желтый
        _FireIntensity ("Fire Intensity", Range(0, 1)) = 0.3 // Интенсивность огня
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DistortionTex;
            float _DistortionStrength;
            float _TimeMultiplier;
            float _FireDirection; // Направление огня в градусах
            fixed4 _Color1;
            fixed4 _Color2;
            fixed4 _Color3;
            float _FireIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Основная текстура
                float2 uv = i.uv;

                // Перевод направления из градусов в радианы
                float radDirection = radians(_FireDirection);

                // Вычисление смещения по направлению
                float2 direction = float2(cos(radDirection), sin(radDirection));

                // Анимация на основе времени
                float timeOffset = _TimeMultiplier * _Time.y;
                uv += direction * sin(uv.x * 10 + timeOffset) * _DistortionStrength;

                // Текстура искажения
                fixed4 distortion = tex2D(_DistortionTex, i.uv);
                uv += (distortion.rg - 0.5) * _DistortionStrength;

                // Градиент огня по направлению
                float gradient = frac(dot(uv, direction) * 0.5 + timeOffset * 0.2);
                fixed4 fireColor = lerp(_Color1, _Color2, gradient);
                fireColor = lerp(fireColor, _Color3, smoothstep(0.3, 0.7, gradient));

                // Снижение интенсивности огненных цветов
                fireColor *= _FireIntensity;

                // Итоговый цвет с основной текстурой
                fixed4 baseColor = tex2D(_MainTex, uv);
                fixed4 finalColor = lerp(baseColor, baseColor * fireColor, _FireIntensity);

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent"
}
