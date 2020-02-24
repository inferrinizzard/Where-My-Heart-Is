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

			float _Radius;
			float _Intensity;
			sampler2D _MainTex;

			sampler2D _GlowMap;
			float4 _GlowMap_TexelSize;
			sampler2D _CameraDepthTexture;
			// o.projPos = ComputeScreenPos(o.vertex);      

			float4 gaussianBlur(sampler2D tex, float2 dir, float dist, float2 uv, float res)
			{
				//this will be our RGBA sum
				float4 sum = float4(0, 0, 0, 0);
				
				//the amount to blur, i.e. how far off center to sample from 
				//1.0 -> blur by one pixel
				//2.0 -> blur by two pixels, etc.
				float blur = dist / res; 
				
				//the direction of our blur
				//(1.0, 0.0) -> x-axis blur
				//(0.0, 1.0) -> y-axis blur
				float hstep = dir.x;
				float vstep = dir.y;
				
				//apply blurring, using a 9-tap filter with predefined gaussian weights
				
				sum += tex2Dlod(tex, float4(uv.x - 4 * blur * hstep, uv.y - 4.0 * blur * vstep, 0, 0)) * 0.0162162162;
				sum += tex2Dlod(tex, float4(uv.x - 3.0 * blur * hstep, uv.y - 3.0 * blur * vstep, 0, 0)) * 0.0540540541;
				sum += tex2Dlod(tex, float4(uv.x - 2.0 * blur * hstep, uv.y - 2.0 * blur * vstep, 0, 0)) * 0.1216216216;
				sum += tex2Dlod(tex, float4(uv.x - 1.0 * blur * hstep, uv.y - 1.0 * blur * vstep, 0, 0)) * 0.1945945946;
				
				sum += tex2Dlod(tex, float4(uv.x, uv.y, 0, 0)) * 0.2270270270;
				
				sum += tex2Dlod(tex, float4(uv.x + 1.0 * blur * hstep, uv.y + 1.0 * blur * vstep, 0, 0)) * 0.1945945946;
				sum += tex2Dlod(tex, float4(uv.x + 2.0 * blur * hstep, uv.y + 2.0 * blur * vstep, 0, 0)) * 0.1216216216;
				sum += tex2Dlod(tex, float4(uv.x + 3.0 * blur * hstep, uv.y + 3.0 * blur * vstep, 0, 0)) * 0.0540540541;
				sum += tex2Dlod(tex, float4(uv.x + 4.0 * blur * hstep, uv.y + 4.0 * blur * vstep, 0, 0)) * 0.0162162162;

				return float4(sum.rgb, 1.0);
			}


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
