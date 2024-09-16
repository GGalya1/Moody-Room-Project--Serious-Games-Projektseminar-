Shader "Custom/ToonWithOutlinesShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Brightness("Brightness", Range(0,1)) = 0.3
        _Strength("Strength", Range(0,1)) = 0.5
        _Color("Color", Color) = (1,1,1,1)
        _Detail("Detail", Range(0,1)) = 0.3

        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width", Range(0.0, 0.1)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        // Пасс для обводки
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "Always" }
            Cull Front

            CGPROGRAM
            #pragma vertex vertOutline
            #pragma fragment fragOutline
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float _OutlineWidth;
            fixed4 _OutlineColor;

            v2f vertOutline (appdata v)
            {
                v2f o;
                // Преобразование нормали в мировые координаты
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                // Смещение позиции вдоль нормали
                float3 pos = mul(unity_ObjectToWorld, v.vertex).xyz + worldNormal * _OutlineWidth;
                o.pos = UnityWorldToClipPos(pos);
                return o;
            }

            fixed4 fragOutline(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        // Пасс для мультяшного шейдинга
        Pass
        {
            Name "ToonShading"
            Tags { "LightMode" = "ForwardBase" }
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldNormal : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Brightness;
            float _Strength;
            fixed4 _Color;
            float _Detail;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Получение цвета текстуры
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                // Вычисление освещения
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0.0, dot(normalize(i.worldNormal), lightDir));
                float toonShade = floor(NdotL / _Detail) * _Detail;

                col.rgb *= toonShade * _Strength + _Brightness;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}