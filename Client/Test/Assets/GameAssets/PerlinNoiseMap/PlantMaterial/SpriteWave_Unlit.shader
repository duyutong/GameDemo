Shader "Custom/SpriteWave_Unlit"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Amplitude("Amplitude", Float) = 0.1
        _Frequency("Frequency", Float) = 2.0
        _Speed("Speed", Float) = 1.0
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

        LOD 100
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _Amplitude;
            float _Frequency;
            float _Speed;

            v2f vert(appdata_t v)
            {
                v2f o;
                float wave = sin(v.vertex.x * _Frequency + _Time.y * _Speed) * _Amplitude;
                v.vertex.y += wave;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}