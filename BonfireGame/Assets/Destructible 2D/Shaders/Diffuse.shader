Shader "Destructible 2D/Diffuse"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		
		// D2D
		[PerRendererData] _AlphaTex ("Alpha Tex", 2D) = "white" {}
		[PerRendererData] _AlphaScale ("Alpha Scale", Vector) = (1,1,0,0)
		[PerRendererData] _AlphaOffset ("Alpha Offset", Vector) = (0,0,0,0)
		[PerRendererData] _AlphaSharpness ("Alpha Sharpness", Float) = 1.0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert nofog keepalpha
		#pragma multi_compile _ PIXELSNAP_ON

		sampler2D _MainTex;
		float4    _Color;
		
		// D2D
		sampler2D _AlphaTex;
		float     _AlphaSharpness;
		float2    _AlphaScale;
		float2    _AlphaOffset;

		struct Input
		{
			float2 uv_MainTex;
			fixed4 color;
			// D2D
			float2 alphaUv;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			#if defined(PIXELSNAP_ON) && !defined(SHADER_API_FLASH)
			v.vertex = UnityPixelSnap (v.vertex);
			#endif
			v.normal = float3(0,0,-1);
			
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = v.color * _Color;
			
			// D2D
			o.alphaUv = (v.texcoord - _AlphaOffset) * _AlphaScale;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 mainTex  = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			float4 alphaTex = tex2D(_AlphaTex, IN.alphaUv);
			
			// Apply alpha tex
			o.Alpha = mainTex.a * saturate(0.5f + (alphaTex.a - 0.5f) * _AlphaSharpness);
			
			// Premultiply alpha
			o.Albedo = mainTex.xyz * o.Alpha;
		}
		ENDCG
	}
	
	Fallback "Transparent/VertexLit"
}