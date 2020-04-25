Shader "Bird/Mask"
{
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Background ("Texture", 2D) = "white" {}
	}
	SubShader {
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _BirdMask;
			sampler2D _Background;
			sampler2D _MainTex;

			fixed4 frag (v2f_img i) : SV_Target {
				float4 mask =  tex2D(_BirdMask, i.uv);
				if(mask.r > 0 || mask.g > 0 || mask.b > 0) {
					// float alpha = 1 - saturate(pow(1 - mask.b, 3));
					// return float4(tex2D(_Background, i.uv + float2(_Time.x, _Time.x)).rgb, alpha) + float4(tex2D(_MainTex, i.uv).rgb, 1 - alpha);
					return float4(tex2D(_Background, i.uv + float2(_Time.x, _Time.x)).rgb, mask.b);
				}
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
