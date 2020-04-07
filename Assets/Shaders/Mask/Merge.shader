Shader "Mask/Merge"
{
	Properties
	{
		_MainTex ("Real", 2D) = "white" {} // Real 

		_Radius("Width", float) = 10
		_Intensity("Intensity", float) = 2
	}
	SubShader {
		Tags { "Queue" = "Transparent" }

		// No culling or depth
		Cull Off 
		ZWrite Off 
		ZTest Always

		// Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#pragma multi_compile __ MASK
			#pragma multi_compile __ OUTLINE
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

			// sampler2D _CameraDepthNormalsTexture;
			// float4 _CameraDepthNormalsTexture_TexelSize;
			// float _NormalMult;
			// float _NormalBias;
			// float _DepthMult;
			// float _DepthBias;

			// half3 Sample (float2 uv) { return tex2D(_GlowMap, uv).rgb; }

			// half3 SampleBox (float2 uv, float delta) {
				// 	float4 o = _GlowMap_TexelSize.xyxy * float2(-delta, delta).xxyy;
				// 	half3 s = Sample(uv + o.xy) + Sample(uv + o.zy) +
				// 	Sample(uv + o.xw) + Sample(uv + o.zw);
				// 	return s * 0.25;
			// }

			// half3 Prefilter (half3 c) {
				// 	half brightness = max(c.r, max(c.g, c.b));
				// half soft = brightness - _Filter.y;
				// soft = clamp(soft, 0, _Filter.z);
				// soft *= soft * _Filter.w;
				// half contribution = max(soft, brightness - _Filter.x) / max(brightness, 0.00001);
				// 	return c * contribution;
			// }

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

				return output;
			}
			
			ENDCG
		}
	}
}
