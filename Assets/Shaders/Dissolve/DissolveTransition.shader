Shader "Dissolve/Transition"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TransitionTex("Transition Texture", 2D) = "white" {}
		_BackgroundTex("Background", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0, 2)) = 0
	}

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _TransitionTex;
			sampler2D _MainTex;
			sampler2D _BackgroundTex;
			float _Cutoff;

			fixed4 frag(v2f_img i) : SV_Target
			{
				fixed4 transit = tex2D(_TransitionTex, i.uv);
				fixed2 direction = float2(0, 0);
				fixed4 col = tex2D(_MainTex, i.uv + _Cutoff * direction);

				if (transit.b < _Cutoff)
				return float4(tex2D(_BackgroundTex, i.uv).rgb, 1 - (_Cutoff - transit.b) / transit.b); // fade with alpha
				// return col = lerp(col, tex2D(_BackgroundTex, i.uv), (_Cutoff - transit.b) / transit.b);

				return float4(col.rgb,  (_Cutoff - transit.b) / transit.b);
			}					
			ENDCG
		}
	}
}
