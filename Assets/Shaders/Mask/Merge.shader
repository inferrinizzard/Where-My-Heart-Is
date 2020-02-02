Shader "Mask/Merge"
{
	Properties
	{
		_Mask("Mask", 2D) = "white" {}
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

			sampler2D _Mask;
			sampler2D _Real;

			fixed4 frag (v2f_img i) : SV_Target {
				return tex2D(_Mask, i.uv).r > .9 ? tex2D(_Real, i.uv) : fixed4(0,0,0,0);
			}
			ENDCG
		}
	}
}
