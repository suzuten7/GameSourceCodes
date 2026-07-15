Shader "Unlit/WTFtest"
{
    Properties
    {
        _Alpha("Alpha",Float) = 1
        _EmColor("EmColor", Float) = 1
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100
        Pass // 2
        {
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            float _Alpha;
            float _EmColor;
            half4 _BaseColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                float em = _EmColor;
            if (i.color.r > em)em = i.color.r;
            if (i.color.g > em)em = i.color.g;
            if (i.color.b > em)em = i.color.b;
            i.color.r /= em;
            i.color.g /= em;
            i.color.b /= em;
            // apply base color
            i.color.a *= _Alpha;
            if (i.color.a > 1)i.color.a = 1;
            if (i.color.a < 0)i.color.a = 0;
            return i.color;
        }
        ENDCG
    }
    }
}
