Shader "Unlit/Gabu_n"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
        _OutlineColor("Color", Color) = (1, 1, 1, 1)
        _OutlineWidth("Width", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass // 1
        {
            cull Front
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            half _OutlineWidth;
            half4 _OutlineColor;
            half4 _BaseColor;

            v2f vert(appdata v)
            {
                // just make a copy of incoming vertex data but scaled according to normal direction
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float2 norm = mul((float3x3)unity_WorldToObject, v.normal);
                norm = normalize(norm);
                o.pos.xy += norm * o.pos.z * _OutlineWidth;
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                // apply outline color
                fixed4 outCol = lerp(i.color, _OutlineColor, _OutlineWidth);
                // apply base color
                return outCol * _BaseColor;
            }
            ENDCG
        }
        
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

            half4 _BaseColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                // apply base color
                return i.color * _BaseColor;
            }
            ENDCG
        }
    }
}
