Shader "Screen/Ripple"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_BiasTex ("Bias Texture", 2D) = "white" {}
		_Offset("Offset", float) = 0
		_Width ("Displacement Width", float) = 0
		_Strength ("Displacement Strength", float) = 0
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

				float _Offset;
				float _Width;
				float _Strength;
				sampler2D _BiasTex;

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
					float uDif = abs(i.uv.x - 0.5);
					float vDif = abs(i.uv.y - 0.5);
					float sqrDist = sqrt((uDif * uDif) + (vDif * vDif));
					float diffFromOffset = abs(_Offset - sqrDist);
					//float diffFromOffset = _Offset - sqrDist;

					fixed4 col;

					if (abs(diffFromOffset) > _Width)
					{
						col = tex2D(_MainTex, i.uv + i.uv * (_Width/diffFromOffset) * _Strength * tex2D(_BiasTex, i.uv));
					}
					else
					{
						col = tex2D(_MainTex, i.uv);
					}

					//col = float4(((_Width / diffFromOffset) * _Strength), 0, 0, 1);
					return col;
				}
				ENDCG
			}
		}
}
