Shader "Mask/Merge"
{
	Properties
	{
		_MainTex ("Real", 2D) = "white" {} // Real 

		_Intensity("Intensity", float) = 2

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
			// #pragma multi_compile __ BLOOM

			#include "UnityCG.cginc"
			// #include "../GaussianBlur.cginc"

			sampler2D _Mask;
			sampler2D _MainTex; // _Real
			sampler2D _Heart;

			sampler2D _GlowMap;
			float4 _GlowMap_TexelSize;
			// sampler2D _CameraDepthTexture;
			// float _Radius;
			float _Intensity;
			// float4 _Filter;

			sampler2D _CameraDepthNormalsTexture;
			float4 _CameraDepthNormalsTexture_TexelSize;
			static const float4x2 dirs = { 0, 1, 1, 0, 0, -1, -1, 0 };

			float4 _DepthOutlineColour;
			float _NormalMult, _NormalBias;
			float _DepthMult, _DepthBias;

			sampler2D _HatchTex;
			int _HatchSize, _HatchSpeed;
			float _Distortion, _NoiseSpeed;

			float _WaveDistance;
			float _WaveTrail;
			float4 _WaveColour;

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
				float4 output;
				#if MASK
					float mask = tex2D(_Mask, i.uv).r;
					output = mask > .5 ? tex2D(_Heart, i.uv) : tex2D(_MainTex, i.uv);
				#else
					output = tex2D(_MainTex, i.uv);
				#endif

				#if OUTLINE
					float4 glow = tex2D(_GlowMap, i.uv);
					// return glow;
					if(glow.a == 0) {
						int NumberOfIterations = 9;
						
						//split texel size into smaller words
						float TX_x = _GlowMap_TexelSize.x;
						float TX_y = _GlowMap_TexelSize.y;
						
						//and a final intensity that increments based on surrounding intensities.
						float4 ColorIntensityInRadius = float4(0, 0, 0, 0);
						
						for(int k = 0; k < NumberOfIterations; k++) {
							for(int j = 0; j < NumberOfIterations; j++) {
								//increase our output color by the pixels in the area
								ColorIntensityInRadius += tex2D(_GlowMap, i.uv.xy + 
								float2((k - NumberOfIterations / 2) * TX_x, (j - NumberOfIterations / 2) * TX_y));
							}
						}

						#if MASK
							if(mask > .5 && ColorIntensityInRadius.a > 0) return ColorIntensityInRadius * _Intensity; 
						#endif

						output += ColorIntensityInRadius * _Intensity;
					}
				#endif

				#if BOIL || WAVE
					float4 depthNormal = tex2D(_CameraDepthNormalsTexture, i.uv);

					float3 normal;
					float depth;
					DecodeDepthNormal(depthNormal, depth, normal);
				#endif

				#if BOIL
					float texIndex = floor(_Time.y * _Speed % 9);
					float row = 1 + texIndex % (_Size - 1);
					float col = floor(texIndex / _Size);
					
					// save depth here and use to reduce outline size / don't show after certain distance

					//get depth as distance from camera in units 
					float boilDepth *= _ProjectionParams.z;

					float depthDifference = 0;
					float normalDifference = 0;

					for(int j = 0; j < 4; j++) Compare(depthDifference, normalDifference, boilDepth, normal, i.uv, dirs[j]);

					depthDifference *= _DepthMult;
					depthDifference = saturate(depthDifference);
					depthDifference = pow(depthDifference, _DepthBias);

					normalDifference *= _NormalMult;
					normalDifference = saturate(normalDifference);
					normalDifference = pow(normalDifference, _NormalBias);

					float outline = normalDifference + depthDifference;
					float4 color = lerp(output, _Colour, outline);
					if(!rgbEquals(output, color)) output *= tex2D(_HatchTex, modUV(i.uv * _Size, row, col, _Size));
				#endif

				#if WAVE
					float waveDepth = Linear01Depth(depth);
					waveDepth = waveDepth * _ProjectionParams.z;

					if(waveDepth < _ProjectionParams.z) {

						float waveFront = step(depth, _WaveDistance);
						float waveTrail = smoothstep(_WaveDistance - _WaveTrail, _WaveDistance, depth);
						float wave = waveFront * waveTrail;

						output = lerp(output, _WaveColour, wave);
					}
				#endif

				#if BIRD
					float4 mask =  tex2D(_BirdMask, i.uv);
					if(mask.r > 0 || mask.g > 0 || mask.b > 0) return float4(tex2D(_Background, i.uv + float2(_Time.x, _Time.x)).rgb, mask.b);
				#endif

				return output;
			}
			
			ENDCG
		}
	}
}
