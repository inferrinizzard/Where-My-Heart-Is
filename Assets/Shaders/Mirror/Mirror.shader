Shader "Mirror/Mirror"
{
	Properties
	{
		_ReflectionTex("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 100

		/*Pass {
			Stencil
			{
				Ref 1
				Comp Always
				Pass Replace
			}
		}*/

		Pass {
			/*Stencil
			{
				Ref 1
				Comp Equal
			}*/

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;
			};

			sampler2D _ReflectionTex;
			float4 _MainTex_ST;

			v2f vert (appdata_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = v.texcoord.xy;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 coords = i.screenPos.xy / i.screenPos.w;
				coords.x = 1 - coords.x;

				// sample the texture
				fixed4 col = tex2D(_ReflectionTex, coords);
				//fixed4 col = tex2D(_ReflectionTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
