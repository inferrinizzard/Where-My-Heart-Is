Shader "Screen/Main"
{
	Properties
	{
		[HideInInspector] _MainTex ("Real", 2D) = "white" {} // Real 

		[Header(Glow)]
		_Radius("Glow Width", float) = 5
		_Intensity("Glow Intensity", float) = 2

		[Header(Boil)]
		_HatchTex ("Hatch Texture", 2D) = "white" {}
		_HatchSize ("Size", Int) = 20
		_HatchSpeed ("Speed", Int) = 5
		_Distortion ("Distortion Amount", Range(1, 1000)) = 1000
		_NoiseSpeed ("Distortion Rate", Range(1, 500)) = 200
		_DepthOutlineColour ("Outline Color", Color) = (0.1, 0, 0.1, 1)
		_NormalMult ("Normal Outline Multiplier", Range(0, 4)) = 0.4
		_NormalBias ("Normal Outline Bias", Range(1, 4)) = 4
		_DepthMult ("Depth Outline Multiplier", Range(0, 4)) = 1
		_DepthBias ("Depth Outline Bias", Range(1, 4)) = 1.6

		[Header(Wave)]
		_WaveTrail ("Length of the trail", Range(0,5)) = 1
		_WaveColour ("Colour", Color) = (1, 1, 1, 1)
		[HideInInspector] _WaveDistance ("Distance from player", float) = 0

		[Header(Fog)]
		_FogExponent("Fog Distance Exponent", float) = 4
		_FogColor("Fog Color", Color) = (1, 1, 1, 1)

		_BirdBackground ("Bird Background", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue" = "Transparent" }

		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		// Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
			#pragma exclude_renderers gles
			#pragma vertex vert_img
			#pragma fragment frag

			#pragma multi_compile __ MASK
			#pragma multi_compile __ OUTLINE
			#pragma multi_compile __ BOIL
			#pragma multi_compile __ WAVE
			#pragma multi_compile __ BIRD
			#pragma multi_compile __ FOG

			#include "UnityCG.cginc"
			// #include "../GaussianBlur.cginc"
			#include "glow.cginc"
			#include "boil.cginc"
			#include "wave.cginc"
			#include "bird.cginc"
			#include "fog.cginc"

			sampler2D _Mask;
			sampler2D _MainTex; // _WaveTrail
			sampler2D _Heart;

			// sampler2D _CameraDepthTexture;

			// fixed4 alphaBlend(fixed4 dst, fixed4 src) {
				// 	fixed4 result = fixed4(0, 0, 0, 0);
				// 	result.a = src.a + dst.a * (1 - src.a);
				// 	if (result.a != 0) result.rgb = (src.a + dst.rgb * dst.a * (1 - src.a)) / result.a;
				// 	return result;
			// }

			fixed4 frag (v2f_img i) : SV_Target {
				int exit = 0;
				float4 output;
				float mask = tex2D(_Mask, i.uv).r;

				#if MASK
					output = mask > .5 ? mask * tex2D(_Heart, i.uv) + (1 - mask) * tex2D(_MainTex, i.uv) : tex2D(_MainTex, i.uv);
					//output = mask * tex2D(_Heart, i.uv) + (1 - mask) * tex2D(_MainTex, i.uv);
				#else
					output = tex2D(_MainTex, i.uv);
				#endif

				// #if OUTLINE
				// 	exit = CalculateGlow(output, i.uv, mask);
				// 	if(exit) return output;
				// #endif

				#if BOIL
					exit = CalculateBoil(output, i.uv, mask);
					if(exit) return output;
				#endif

				#if WAVE
					exit = CalculateWave(output, i.uv, mask);
					if(exit) return output;
				#endif

				#if BIRD
					exit = CalculateBird(output, i.uv, mask);
					if(exit) return output;
				#endif

				#if FOG
					exit = CalculateFog(output, i.uv, mask);
					if(exit) return output;
				#endif

				return output;
			}
			
			ENDCG
		}
	}
}
