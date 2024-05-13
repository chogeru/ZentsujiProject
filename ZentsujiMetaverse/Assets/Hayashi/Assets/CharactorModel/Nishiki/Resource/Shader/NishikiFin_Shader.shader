Shader "Nishiki/NishikiFin_Shader"
{
    Properties
    {
        [MaterialToggle] _CustomizeColor("CustomizeColor",int) = 0
        _CustomColor("CustomColor",color) = (1,1,1,1)
        [Space(10)]

        _Metallic("MetallicValue",Range(0,1)) = 0
        _Roughness("RoughnessValue",Range(0,1)) = 0.6
        [Space(30)]

        [NoScaleOffset] _AlphaMap("AlphaMap",2D) = "white"
        [NoScaleOffset] _BaseColorMap("BaseColorMap",2D) = "white"
        [NoScaleOffset] _NormalMap("NormalMap",2D) = "bump"
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200
        Cull Off

        CGPROGRAM
        
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        sampler2D _AlphaMap;
        sampler2D _BaseColorMap;
        sampler2D _NormalMap;

        int _CustomizeColor;

        fixed3 _CustomColor;
        half _Metallic;

        half _Roughness;


        struct Input
        {
            float2 uv_AlphaMap;
        };


        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_AlphaMap;

            fixed3 color = _CustomizeColor ? _CustomColor : tex2D(_BaseColorMap, uv);

            o.Albedo = color;
            o.Metallic = _Metallic;
            o.Smoothness = 1 - _Roughness;
            o.Alpha = tex2D(_AlphaMap,uv).r;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
