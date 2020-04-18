Shader "Outline/Depth"
{
	Properties {
		[HideInInspector] _MainTex ("Texture", 2D) = "white" {}
		_HatchTex ("Hatch Texture", 2D) = "white" {}
		_Size ("Size", Int) = 3
		_Speed ("Speed", Int) = 10
		_Distortion ("Distortion Amount", Range(1, 1000)) = 700
		_NoiseSpeed ("Distortion Rate", Range(1, 500)) = 400
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
			sampler2D _HatchTex;
			int _Size, _Speed;
			float _Distortion, _NoiseSpeed;

			sampler2D _CameraDepthTexture;
			float4 _CameraDepthTexture_TexelSize;
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

				// float depth2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv + _CameraDepthTexture_TexelSize.xy * offset);

				neighborDepth *= _ProjectionParams.z;

				depthOutline += baseDepth - neighborDepth;
				// depthOutline += (baseDepth - neighborDepth) * depth2;

				float3 normalDifference = baseNormal - neighborNormal;
				normalDifference = normalDifference.r + normalDifference.g + normalDifference.b;
				normalOutline += normalDifference;
			}

			fixed4 alphaBlend(fixed4 dst, fixed4 src) {
				fixed4 result = fixed4(0, 0, 0, 0);
				result.a = src.a + dst.a * (1 - src.a);
				if (result.a != 0) result.rgb = (src.a + dst.rgb * dst.a * (1 - src.a)) / result.a;
				return result;
			}

			bool rgbEquals(fixed4 a, fixed4 b){
				// return a.r == b.r && a.g == b.g && a.b == b.b;
				if(abs(a.r - b.r) > .1) return false;
				if(abs(a.g - b.g) > .1) return false;
				if(abs(a.b - b.b) > .1) return false;
				return true;
			}

			float2 modUV(float2 uv, float row, float col, int size) {
				row = row % size;
				col = col % size;
				return (uv / size) + float2(row/size, col/size);
			}
			
			fixed4 frag (v2f_img i) : SV_Target {
				// distortion but outliness are pos-locked for now
				// float2 noiseUV = i.uv + tex2D(_HatchTex, i.pos.xy / _Distortion  + float2((_Time.w % 10) / 10, (_Time.w % 10) / 10)) / _NoiseSpeed;
				// // return tex2D(_MainTex, noiseUV);

				// float texIndex = floor(_Time.y * _Speed % 9);
				// float row = 1 + texIndex % (_Size - 1);
				// float col = floor(texIndex / _Size);
				// // return tex2D(_HatchTex, modUV(noiseUV * _Size, row, col, _Size));

				float texIndex = floor(_Time.y * _Speed % 9);
				float row = 1 + texIndex % (_Size - 1);
				float col = floor(texIndex / _Size);
				
				float4 depthNormal = tex2D(_CameraDepthNormalsTexture, i.uv);

				float3 normal;
				float depth;
				// float depth2 = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				DecodeDepthNormal(depthNormal, depth, normal);

				// save depth here and use to reduce outline size / don't show after certain distance

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
				float4 source = tex2D(_MainTex, i.uv);
				float4 color = lerp(source, _Colour, outline);
				if(rgbEquals(source, color)) return source;
				// if(rgbEquals(color, _Colour) || rgbEquals(lerp(color, _Colour, outline), color)) return 1;
				// color = alphaBlend(color, tex2D(_HatchTex, i.uv * 3));
				color *= tex2D(_HatchTex, modUV(i.uv * _Size, row, col, _Size));
				return color;
			}

			ENDCG
		}
	}
}
