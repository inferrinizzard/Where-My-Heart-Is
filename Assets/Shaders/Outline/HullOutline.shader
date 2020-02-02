Shader "Custom/HullOutline" {
	Properties {
		_Color ("Tint", Color) = (0, 0, 0, 1)
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}

		_OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
		_OutlineThickness ("Outline Thickness", Range(0,1)) = 0.1
	}
	SubShader {
		Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;

		struct Input {
			float2 uv_MainTex; 
		};

		void surf (Input i, inout SurfaceOutputStandard o) {
			o.Albedo = _Color;
		}
		ENDCG

		Pass{
			Cull Front

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			fixed4 _OutlineColor;
			float _OutlineThickness;

			struct appdata {
				float4 vertex : POSITION;
				float4 normal : NORMAL;
			};

			struct v2f { float4 position : SV_POSITION; };

			v2f vert(appdata v) {
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex + normalize(v.normal) * _OutlineThickness);
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET { return _OutlineColor; }

			ENDCG
		}

	}
	FallBack "Standard"
}