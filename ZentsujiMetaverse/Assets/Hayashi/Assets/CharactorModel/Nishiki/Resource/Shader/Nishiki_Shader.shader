Shader "Nishiki/Nishiki_Shader"
{
    Properties
    {
        [MaterialToggle] _CustomizeColor("CustomizeColor",int) = 0
        [MaterialToggle] _CustomizeMetallic("CustomizeMetallic",int) = 0
        [MaterialToggle] _CustomizeRoughness("CustomizeRoughness",int) = 0
        [Space(20)]
        
        _CustomBaseColor("CustomBasecolor",color) = (1,1,1,1)
        _CustomPatternColor1("CustomPatternColor1",color) = (1,0,0,1)
        _CustomPatternColor2("CustomPatternColor2",color) = (0,0,0,1)
        _CustomGrungeIntensity("CustomGrungeIntensity",Range(0,1)) = 0.8
        [Space(10)]

        _PatternOffsetX("PatternOffset X",float) = 0
        _PatternOffsetY("PatternOffset Y",float) = 0
        _PatternScale("PatternScale",float) = 1
        [Space(20)]

        _CustomMetallic("CustomMetallicValue",Range(0,1)) = 0
        _CustomRoughness("CustomRoughnessValue",Range(0,1)) = 0.3
        [Space(50)]

        [NoScaleOffset] _BaseColorMap("BaseColorMap",2D) = "white" {}
        [NoScaleOffset] _MetallicMap("MetallicMap",2D) = "black" {}
        [NoScaleOffset] _RoughnessMap("RoughnessMap",2D) = "white" {}
        [NoScaleOffset] _NormalMap("NormalMap",2D) = "bump" {}
        [Space(10)]

        [NoScaleOffset] _GrungeMask("GrungeMask",2D) = "white" {}
        [NoScaleOffset] _PatternMask1("PatternMask1",2D) = "black"{}
        [NoScaleOffset] _PatternMask2("PatternMask2",2D) = "black"{}
        [NoScaleOffset] _PatternMap("PatternMap",2D) = "white"{}
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        sampler2D _BaseColorMap;
        sampler2D _RoughnessMap;
        sampler2D _MetallicMap;
        sampler2D _NormalMap;

        sampler2D _GrungeMask;
        sampler2D _PatternMap;
        sampler2D _PatternMask1;
        sampler2D _PatternMask2;

        int _CustomizeColor;
        int _CustomizeMetallic;
        int _CustomizeRoughness;

        fixed3 _CustomBaseColor;
        fixed3 _CustomPatternColor1;
        fixed3 _CustomPatternColor2;

        half _CustomGrungeIntensity;

        half _PatternOffsetX;
        half _PatternOffsetY;
        half _PatternScale;

        half _CustomMetallic;
        half _CustomRoughness;

        struct Input
        {
            float2 uv_BaseColorMap;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        fixed3 CustomizeColor(float2 uv) 
        {
            float2 offsetuv = uv + float2(_PatternOffsetX, _PatternOffsetY);
            
            offsetuv = 0.5 - offsetuv;
            offsetuv *= _PatternScale;
            offsetuv = 0.5 + offsetuv;

            fixed3 baseColor = _CustomBaseColor;
            fixed3 patternColor1 = _CustomPatternColor1;
            fixed3 patternColor2 = _CustomPatternColor2;

            half patternMap = tex2D(_PatternMap, uv).r;
            half grungeMask = tex2D(_GrungeMask, offsetuv).r;
            half patternMask1 = tex2D(_PatternMask1, offsetuv).r;
            half patternMask2 = tex2D(_PatternMask2, offsetuv).r;

            patternMask1 *= patternMap;
            patternMask2 *= patternMap;

            fixed3 color = lerp(baseColor, patternColor1, patternMask1);
            color = lerp(color, patternColor2, patternMask2);

            half grunge = lerp(_CustomGrungeIntensity, 1, grungeMask).r;

            color *= grunge;

            color = lerp(color, tex2D(_BaseColorMap, uv), tex2D(_MetallicMap, uv).r);

            return color;
        }
        

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_BaseColorMap;

            fixed3 defaultColor = tex2D(_BaseColorMap, uv);
            half defaultMetallic = tex2D(_MetallicMap, uv);
            half defaultRoughness = tex2D(_RoughnessMap, uv);

            fixed3 color = _CustomizeColor ? CustomizeColor(uv) : defaultColor;
            half metallic = _CustomizeMetallic ? _CustomMetallic : defaultMetallic;
            half roughness = 1 - (_CustomizeRoughness ? _CustomRoughness : defaultRoughness);

            o.Albedo = color;
            o.Smoothness = roughness;
            o.Metallic = metallic;
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_BaseColorMap));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
