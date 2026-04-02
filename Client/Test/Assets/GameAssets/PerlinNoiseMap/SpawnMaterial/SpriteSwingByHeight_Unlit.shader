Shader "Custom/SpritePlantWind_Simple"
{
    Properties
    {
        _MainTex ("Sprite Texture（精灵贴图）", 2D) = "white" {}

        // =========================
        // 🌿 基础摆动（函数风回退）
        // =========================
        _Amplitude ("Main Bend Strength（主弯曲 / 树干）", Float) = 0.15
        _DetailAmplitude ("Detail Bend Strength（细节抖动 / 叶子）", Float) = 0.05
        _Speed ("Wind Speed（函数风速度）", Float) = 1.0
        _Frequency ("Detail Frequency（细节频率）", Float) = 3.0
        _Stiffness ("Stiffness（底部刚性）", Float) = 2.0
        _WindDir ("Wind Direction（风向）", Float) = 1.0

        // =========================
        // 🌬️ 风贴图
        // =========================
        _WindTex ("Wind Noise Texture（灰度图）", 2D) = "black" {}
        _WindScale ("Wind Scale（空间缩放）", Float) = 0.1
        _WindSpeed ("Wind Speed(风速)",Float) = 0.1
        _UseWindTex ("Use Wind Texture（0 = 函数风 / 1 = 贴图风）", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _WindTex;

            float _Amplitude;
            float _DetailAmplitude;
            float _Speed;
            float _Frequency;
            float _Stiffness;
            float _WindDir;
            float _WindSpeed;

            float _WindScale;
            float _UseWindTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color  : COLOR;
            };

            // -------------------------
            // 简单hash函数，用于每棵植物随机
            // -------------------------
            float hash(float n)
            {
                return frac(sin(n) * 43758.5453);
            }

            v2f vert(appdata v)
            {
                v2f o;
                float3 pos = v.vertex.xyz;

                // -------------------------
                // 高度影响（底部更稳）
                // -------------------------
                float height01 = v.uv.y;
                float bendFactor = pow(height01, _Stiffness);

                // -------------------------
                // 世界坐标，用于随机 & 风贴图
                // -------------------------
                float3 worldPos = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
                float rand = hash(worldPos.x * 12.9898 + worldPos.y * 78.233);

                float wind = 0;

                if (_UseWindTex > 0.5)
                {
                    // =========================
                    // 风场贴图模式
                    // =========================
                    float phaseOffset = rand * 1000; // 每棵植物不同的相位偏移
                    float2 windUV = worldPos.xz * _WindScale;
                    float uOffset = _Time.y * _WindSpeed + phaseOffset; // 每秒偏移的 UV
                    windUV.x += uOffset;
                    windUV = frac(windUV); // 循环

                    float windSample = tex2Dlod(_WindTex, float4(windUV, 0, 0)).r;
                    wind = windSample * 2 -1;
                }
                // =========================
                // 函数风
                // =========================
                float mainWave = sin(_Time.y * _Speed + rand * 6.28);
                float detailWave = sin(_Time.y * (_Speed * 2.0) + pos.y * _Frequency + rand * 10.0);
                wind += mainWave * _Amplitude + detailWave * _DetailAmplitude;

                // -------------------------
                // 应用风到顶点
                // -------------------------
                pos.x += wind * bendFactor * _WindDir;

                o.vertex = UnityObjectToClipPos(float4(pos,1));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv) * i.color;
            }

            ENDCG
        }
    }
}