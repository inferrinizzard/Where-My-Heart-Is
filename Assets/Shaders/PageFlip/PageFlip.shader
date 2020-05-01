// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Transition/PageFlip"
{
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Progress ("Progress", float) = 0
		_A ("Apex", float) = 1
		_Rho ("Rho", float) = 1
		_Theta ("Theta", float) = 1
		_BottomLeft ("Corner", Vector) = (0, 0, 0)
	}
	SubShader {
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float _A, _Rho, _Theta;
			float3 _BottomLeft;

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata_base v) {
				v2f o;
				float3 test = mul(unity_WorldToObject, float4(_BottomLeft, 1));
				float3 vertex = v.vertex;
				vertex.y += distance(vertex, test)/10;
				o.vertex = UnityObjectToClipPos(vertex);
				o.uv = v.texcoord;
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target {
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
