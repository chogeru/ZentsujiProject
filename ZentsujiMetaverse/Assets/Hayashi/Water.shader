Shader "Custom/Water"
{
       Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Cube ("Reflection Cubemap", Cube) = "" {}
        _WaveSpeed ("Wave Speed", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _BumpMap;
        samplerCUBE _Cube;
        float _WaveSpeed;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;

            // ”g“®Œø‰Ê
            float wave = sin(_Time.y * _WaveSpeed + IN.uv_MainTex.x * 10.0) * 0.1;
            half4 bump = tex2D (_BumpMap, IN.uv_BumpMap + wave);
            o.Normal = UnpackNormal(bump);

            // ”½ŽËŒø‰Ê
            half3 worldNormal = normalize(o.Normal);
            half3 worldViewDir = normalize(UnityWorldSpaceViewDir(IN.worldPos));
            half3 worldRefl = reflect(-worldViewDir, worldNormal);
            half4 refl = texCUBE (_Cube, worldRefl);
            o.Emission = refl.rgb * 0.5;
        }
        ENDCG
    }
    FallBack "Diffuse"

}
