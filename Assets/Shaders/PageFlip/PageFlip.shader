// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Transition/PageFlip"
{
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_A ("Apex", Range(0, 10)) = 1
		_Rho ("Rho", Range(0, 1)) = 1
		_Theta ("Theta", Range(0, 1)) = 1
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

			static const float half_PI = 3.141592653589793238462 / 2;
			static const float tau = 6.283185307179586476924;

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata_base v) {
				v2f o;
				float3 bottomLeft = mul(unity_WorldToObject, float4(_BottomLeft, 1));
				float3 vertex = v.vertex;

				float theta = _Theta * half_PI;
				float rho = _Rho * tau;

				float2 lateral = (vertex.xz - bottomLeft.xz) ;

				float R = sqrt(lateral.x * lateral.x + pow(lateral.y + _A, 2));
				float r = R * sin(theta);
				float beta = asin(lateral.x / R) / sin(theta);

				float3 vertex2 = float3(r * sin(beta), R + _A - r * (1 - cos(beta)) * sin(theta) , r * (1 - cos(beta)) * cos(theta));
				float3 vertex3 = float3(vertex2.x * cos(rho) - vertex2.z * sin(rho), vertex2.y, vertex2.x * sin(rho) + vertex2.z * cos(rho)).xzy;
				vertex3.y /= 10;

				// o.vertex = UnityObjectToClipPos(vertex);
				o.vertex = UnityObjectToClipPos(vertex3 + bottomLeft - float3(0, 0, _A * 2));
				o.uv = 1 - v.texcoord;
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
