Shader "Custom/TerrainShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Terrain Texture Array", 2DArray) = "white" {}
		_GridLinesTex ("Grid Texture", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types.
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.5 target, for texture array support.
		#pragma target 3.5

		// Add toggleable GRIDLINES_ON command.
		#pragma multi_compile _ GRIDLINES_ON

		// Declare _MainTex's type as that of a 2D texture array.
		UNITY_DECLARE_TEX2DARRAY(_MainTex);

		sampler2D _GridLinesTex;

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		struct Input {
			float4 color : COLOR;
			float3 worldPos;
			float3 terrain;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		float4 GetTerrainColor (Input IN, int index) {
			float3 uvw = float3(IN.worldPos.xz * 0.02, IN.terrain[index]);
			float4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, uvw);
			return c * IN.color[index];
		}

		void vert (inout appdata_full v, out Input data) {
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.terrain = v.texcoord2.xyz;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from texture array.
			// float2 uv = IN.worldPos.xz * 0.02;
			//fixed4 c = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(uv, 0));
			fixed4 c =
				GetTerrainColor(IN, 0) +
				GetTerrainColor(IN, 1) +
				GetTerrainColor(IN, 2);
			// Add the togglable gridlines between cells.
			
			fixed4 gridlines = 1;
			#if defined(GRIDLINES_ON)
				float2 gridlinesUV = IN.worldPos.xz;
				gridlinesUV.x *= 1 / (4 * 8.66025404) / 1.15;
				gridlinesUV.y *= 1 / (2 * 15.0) / 1.15;
				gridlines = tex2D(_GridLinesTex, gridlinesUV);
			#endif
			// Set the albedo.
			o.Albedo = c.rgb * gridlines * _Color ;
			// Metallic and smoothness come from slider variables.
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
