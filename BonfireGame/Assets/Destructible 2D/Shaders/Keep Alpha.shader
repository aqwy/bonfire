// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Destructible 2D/Keep Alpha"
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
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				#pragma multi_compile DUMMY PIXELSNAP_ON
				
				#include "UnityCG.cginc"
				
				sampler2D _MainTex;
				float4    _Color;
				
				// D2D
				sampler2D _AlphaTex;
				float     _AlphaSharpness;
				float2    _AlphaScale;
				float2    _AlphaOffset;
				
				struct a2v
				{
					float4 vertex    : POSITION;
					float4 color     : COLOR;
					float2 texcoord0 : TEXCOORD0;
				};
				
				struct v2f
				{
					float4 vertex    : SV_POSITION;
					float4 color     : COLOR;
					float2 texcoord0 : TEXCOORD0;
					float2 texcoord1 : TEXCOORD1;
				};
				
				void Vert(a2v i, out v2f o)
				{
					o.vertex    = UnityObjectToClipPos(i.vertex);
					o.color     = i.color * _Color;
					o.texcoord0 = i.texcoord0;
					o.texcoord1 = (i.texcoord0 - _AlphaOffset) * _AlphaScale;
#if PIXELSNAP_ON
					o.vertex = UnityPixelSnap(o.vertex);
#endif
				}
				
				void Frag(v2f i, out float4 o:COLOR0)
				{
					float4 mainTex  = tex2D(_MainTex, i.texcoord0);
					float4 alphaTex = tex2D(_AlphaTex, i.texcoord1);
					
					// Multiply the color
					o.rgba = mainTex * i.color;
					
					// Apply alpha tex
					o.a *= saturate(0.5f + (alphaTex.a - 0.5f) * _AlphaSharpness);
					
					// Premultiply alpha
					o.rgb *= o.a;
				}
			ENDCG
		}
	}
}
