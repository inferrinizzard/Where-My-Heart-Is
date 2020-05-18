Shader "Mask/OpenWindowRamp"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RampTex ("Ramp Texture", 2D) = "white" {}
		_MaskCutoff ("Cutoff", float) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _RampTex;
			float _MaskCutoff;
			float4 _RampTex_ST;


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				if (col.r > 0.5)
				{
					col = tex2D(_RampTex, TRANSFORM_TEX(i.uv, _RampTex)) * _MaskCutoff;
				}
				/*if (ramp.r > _MaskCutoff)
				{
					col = float4(0, 0, 0, 0);
				}*/

				return col;
			}
			ENDCG
		}
	}
}
