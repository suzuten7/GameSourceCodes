Shader "Custom/TransparentOutlineShader"
{
    Properties
    {
        _Color("Base Color", Color) = (1,1,1,0)
        _OutlineColor("Outline Color", Color) = (1,0,0,1)
        _Outline("Outline width", Range (0.002, 0.03)) = 0.005
    }

    SubShader
    {
        Tags{"Queue" = "Overlay" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input
        {
            float4 color : COLOR;
        };

        half _Outline;
        fixed4 _OutlineColor;

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Base color with transparency
            fixed4 c = _Color;
            c.a = 0;

            // Calculate the outline
            float2 ddx = ddx(IN.color);
            float2 ddy = ddy(IN.color);
            float alpha = _Outline * 1.5 * (ddx.y * ddy.x - ddx.x * ddy.y);
            clip(alpha - 0.5);

            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
