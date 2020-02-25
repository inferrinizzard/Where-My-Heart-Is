Shader "Mask/Merge"
{
	Properties
	{
		_Dream("Dream", 2D) = "white" {}
		_Real("Real", 2D) = "white" {}

		_Radius("Width", float) = 5
		_Intensity("Intensity", float) = 2
	}
	SubShader {
		Tags { "Queue" = "Transparent" }

		// No culling or depth
		Cull Off 
		ZWrite Off 
		ZTest Always

		//Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#pragma shader_feature OUTLINE_GLOW
			#pragma shader_feature OUTLINE_EDGE
			#pragma shader_feature BLOOM

			#include "UnityCG.cginc"
			#include "../GaussianBlur.cginc"

			sampler2D _Mask;
			sampler2D _Dream;
			sampler2D _Real;

			sampler2D _GlowMap;
			float4 _GlowMap_TexelSize;
			sampler2D _CameraDepthTexture;
			float _Radius;
			float _Intensity;

			sampler2D _CameraDepthNormalsTexture;
			float4 _CameraDepthNormalsTexture_TexelSize;
			float _NormalMult;
			float _NormalBias;
			float _DepthMult;
			float _DepthBias;

			fixed4 frag (v2f_img i) : SV_Target {
				float mask = tex2D(_Mask, i.uv).r;
				float4 output;
				if(mask > .5) {
					output = tex2D(_Real, i.uv);
				}
				else {
					output = tex2D(_Dream, i.uv);
				}

				#if OUTLINE_GLOW || OUTLINE_EDGE
					float4 glow = tex2D(_GlowMap, i.uv);
				#endif

				#if OUTLINE_GLOW
					if(mask > .5)
					{
						float resX = _GlowMap_TexelSize.z;
						float resY = _GlowMap_TexelSize.w;
						float4 blurX = gaussianBlur(_GlowMap, float2(1,0), _Radius, i.uv, resX); //9 lookups
						float4 blurY = gaussianBlur(_GlowMap, float2(0,1), _Radius, i.uv, resY); //9 lookups

						float4 outline = (saturate(blurX + blurY) - glow) * _Intensity;

						output += outline;
					}
				#endif

				#if OUTLINE_EDGE
					
				#endif

				return output;
			}
			ENDCG
		}
	}
}
