Shader "Outline/Glow"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_Radius("Width", float) = 1
		_Intensity("Intensity", float) = 1
	}
	SubShader {
		Tags { "Queue" = "Transparent" }

		Cull Off 
		ZWrite On 
		ZTest Always

		// Blend OneMinusDstColor One

		//Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "../GaussianBlur.cginc"

			float _Radius;
			float _Intensity;
			sampler2D _MainTex;

			sampler2D _GlowMap;
			float4 _GlowMap_TexelSize;
			sampler2D _CameraDepthTexture;
			// o.projPos = ComputeScreenPos(o.vertex);      

			fixed4 frag (v2f_img i) : SV_Target {
				float4 glow = tex2D(_GlowMap, i.uv);
				float resX = _GlowMap_TexelSize.z;
				float resY = _GlowMap_TexelSize.w;
				float4 blurX = gaussianBlur(_GlowMap, float2(1,0), _Radius, i.uv, resX);
				float4 blurY = gaussianBlur(_GlowMap, float2(0,1), _Radius, i.uv, resY);

				float4 outline = (saturate(blurX + blurY) - glow) * _Intensity;

				return tex2D(_MainTex, i.uv) + outline;
			}
			ENDCG
		}
	}
}
