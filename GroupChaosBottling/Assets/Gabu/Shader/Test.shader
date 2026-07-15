Shader "CustomRenderTexture/Test"
{
Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _BeginFade ("Begin Fade Distance", Float) = 0.0
        _EndFade ("End Fade Distance", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 4.0
        struct Input
        {
            fixed2 uv_MainTex;
            fixed distance;
            fixed4 screenPosition;
        };
        uniform sampler2D _MainTex;
        uniform half _Glossiness;
        uniform half _Metallic;
        uniform fixed4 _Color;
        uniform float _BeginFade;
        uniform float _EndFade;
        inline float DitherMatrix(int x, int y)
        {
            const float dm[ 64 ] = {
                 1, 49, 13, 61,  4, 52, 16, 64,
                33, 17, 45, 29, 36, 20, 48, 32,
                 9, 57,  5, 53, 12, 60,  8, 56,
                41, 25, 37, 21, 44, 28, 40, 24,
                 3, 51, 15, 63,  2, 50, 14, 62,
                35, 19, 47, 31, 34, 18, 46, 30,
                11, 59,  7, 55, 10, 58,  6, 54,
                43, 27, 39, 23, 42, 26, 38, 22};
            return dm[y * 8 + x] / 64;
        }
        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.distance = -UnityObjectToViewPos(v.vertex).z;
            o.screenPosition = ComputeScreenPos(UnityObjectToClipPos(v.vertex));
        }
        void surf(Input i, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, i.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            float4 sp = i.screenPosition / i.screenPosition.w;
            sp.xy = sp.xy * _ScreenParams.xy;
            float msk = DitherMatrix(fmod(sp.x, 8), fmod(sp.y, 8));
            clip((i.distance - _ProjectionParams.y - _BeginFade) / (_ProjectionParams.y + _EndFade) - msk);
        }
        ENDCG
    }
}
