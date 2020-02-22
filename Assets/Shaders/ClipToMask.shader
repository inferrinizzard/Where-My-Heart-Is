Shader "Custom/ClipToMask"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Mask ("Mask", 2D) = "white" {}
		_MaskValue("Mask Value", Range (0, 1)) = 1 
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}
		// No culling or depth
		Cull Off 
		ZWrite Off 
		ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

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
			sampler2D _Mask;
			float _MaskValue;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;
				fixed4 mask = tex2D(_Mask, i.uv);
				if (_MaskValue == 1)
				{
					if (mask.r > 0.5)// if the pixel on the mask is the designated true value
					{
						col = tex2D(_MainTex, i.uv);
					}
					else
					{
						col.rgba = float4(0, 0, 0, 0.5);
					}
				}
				else
				{
					if (mask.r < 0.5)// if the pixel on the mask is the designated true value
					{
						col = tex2D(_MainTex, i.uv);
					}
					else
					{
						col.rgba = float4(0, 0, 0, 0.5);
					}
				}
				
				return col;
			}
			ENDCG
		}
	}
}
