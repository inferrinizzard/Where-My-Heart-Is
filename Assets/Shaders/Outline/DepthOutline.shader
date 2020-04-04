Shader "Outline/Depth"
{
	Properties {
		[HideInInspector] _MainTex ("Texture", 2D) = "white" {}
		_Colour ("Outline Color", Color) = (0, 0, 0, 1)
		_NormalMult ("Normal Outline Multiplier", Range(0, 4)) = 1
		_NormalBias ("Normal Outline Bias", Range(1, 4)) = 1
		_DepthMult ("Depth Outline Multiplier", Range(0, 4)) = 1
		_DepthBias ("Depth Outline Bias", Range(1, 4)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Off ZWrite Off ZTest Always

		Pass {
			CGPROGRAM
			#pragma exclude_renderers gles
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _CameraDepthNormalsTexture;
			float4 _CameraDepthNormalsTexture_TexelSize;
			static const float4x2 dirs = { 0, 1, 1, 0, 0, -1, -1, 0 };

			float4 _Colour;
			float _NormalMult, _NormalBias;
			float _DepthMult, _DepthBias;

			void Compare(inout float depthOutline, inout float normalOutline, float baseDepth, float3 baseNormal, float2 uv, float2 offset) {
				//read neighbor pixel
				float4 neighbour = tex2D(_CameraDepthNormalsTexture, uv + _CameraDepthNormalsTexture_TexelSize.xy * offset);
				float3 neighborNormal;
				float neighborDepth;
				DecodeDepthNormal(neighbour, neighborDepth, neighborNormal);
				neighborDepth *= _ProjectionParams.z;

				depthOutline += baseDepth - neighborDepth;

				float3 normalDifference = baseNormal - neighborNormal;
				normalDifference = normalDifference.r + normalDifference.g + normalDifference.b;
				normalOutline += normalDifference;
			}
			
			fixed4 frag (v2f_img i) : SV_Target {
				float4 depthNormal = tex2D(_CameraDepthNormalsTexture, i.uv);

				float3 normal;
				float depth;
				DecodeDepthNormal(depthNormal, depth, normal);

				//get depth as distance from camera in units 
				depth *= _ProjectionParams.z;

				float depthDifference = 0;
				float normalDifference = 0;

				for(int j = 0; j < 4; j++) Compare(depthDifference, normalDifference, depth, normal, i.uv, dirs[j]);

				depthDifference *= _DepthMult;
				depthDifference = saturate(depthDifference);
				depthDifference = pow(depthDifference, _DepthBias);

				normalDifference *= _NormalMult;
				normalDifference = saturate(normalDifference);
				normalDifference = pow(normalDifference, _NormalBias);

				float outline = normalDifference + depthDifference;
				float4 color = lerp(tex2D(_MainTex, i.uv), _Colour, outline);
				return color;
			}
			ENDCG
		}
	}
}
