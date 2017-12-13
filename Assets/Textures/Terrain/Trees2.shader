Shader "Custom/Trees2" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal ", Float) = 1
            _HeightMap ("Height Map", 2D) = "white" {}
        _HeightMapScale ("Height", Float) = 1
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _MetallicGlossMap ("Metallic", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _OcclusionMap("OcclusionMap", 2D) = "white" {}
        _OcclusionStrength("Occlusion Strength", Float) = 1
 
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
     
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert
 
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        #pragma glsl
 
        sampler2D _MainTex;
        sampler2D _OcclusionMap;
        sampler2D _BumpMap;
        sampler2D _MetallicGlossMap;
        sampler2D _HeightMap;
 
        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 worldPos;
        };
 
        half _HeightMapScale;
        half _Glossiness;
        half _Metallic;
        half _BumpScale;
        half _OcclusionStrength;
        fixed4 _Color;
 
        void vert(inout appdata_full v,  out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            float4 heightMap = tex2Dlod(_HeightMap, float4(v.texcoord.xy,0,0));
            //fixed4 heightMap = _HeightMap;
            v.vertex.z += heightMap.b * _HeightMapScale;
        }
 
        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            float2 uv1 = IN.worldPos.xz;
            fixed4 c = tex2D(_MainTex, uv1 * 0.075) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            fixed4 gloss = tex2D(_MetallicGlossMap, IN.uv_MainTex);
            o.Metallic = gloss.r * _Metallic;
            o.Smoothness = gloss.a * _Glossiness;
 
            o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
            o.Occlusion = tex2D(_OcclusionMap, IN.uv_MainTex) * _OcclusionStrength;
 
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}