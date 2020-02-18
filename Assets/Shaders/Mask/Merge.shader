Shader "Mask/Merge"
{
	Properties
	{
		_Mask("Mask", 2D) = "white" {}
		_Dream("Dream", 2D) = "white" {}
		_Real("Real", 2D) = "white" {}
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

			#include "UnityCG.cginc"

			sampler2D _Dream;
			sampler2D _Mask;
			sampler2D _Real;

			sampler2D _GlowMap;

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
				float mask = tex2D(_Mask, i.uv).r;
				float4 output;
				if(mask > .5) {
					output = tex2D(_Real, i.uv);
				}
				else {
					output = tex2D(_Dream, i.uv);
				}

				float4 glow = tex2D(_GlowMap, i.uv);
				// float resX = _GlowMap_TexelSize.z;
				// float resY = _GlowMap_TexelSize.w;
				// float4 blurX = gaussianBlur(_GlowMap, float2(1,0), 1, input.uv, resX);
				// float4 blurY = gaussianBlur(_GlowMap, float2(0,1), 1, input.uv, resY);

				// return blurX + blurY;
				return output + glow;
				// return glow;
			}
			ENDCG
		}
	}
}
