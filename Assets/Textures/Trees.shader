Shader "Custom/Trees" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Standard alpha
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;

        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            float2 uv1 = IN.worldPos.xz;
            float4 noise1 = tex2D(_MainTex, uv1 * 0.025);
             

            fixed4 c = noise1;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = .85;
             o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        }
        ENDCG
    }
    FallBack "Diffuse"
}