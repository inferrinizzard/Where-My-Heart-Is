Shader "Custom/SDF" {
	Properties {
		
	}
	SubShader {
		//the material is completely non-transparent and is rendered at the same time as the other opaque geometry
		Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

		Pass{
			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			struct appdata {
				float4 vertex : POSITION;
			};

			struct v2f{
				float4 position : SV_POSITION;
				float4 worldPos : TEXCOORD0;
			};

			v2f vert(appdata v){
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			float scene(float2 position, float radius) {
				return length(position) - radius;
			}

			fixed4 frag(v2f i) : SV_TARGET{
				float dist = scene(i.worldPos.xz, .5);
				fixed4 col = fixed4(dist, dist, dist, 1);
				return col;
			}

			ENDCG
		}
	}
	FallBack "Standard" //fallback adds a shadow pass so we get shadows on other objects
}