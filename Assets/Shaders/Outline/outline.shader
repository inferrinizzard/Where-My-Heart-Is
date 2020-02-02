Shader "Custom/Outline"{
	Properties{
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Weight ("Thickness", Range(0,3)) = 0.03
	}

	SubShader{
		Cull Off
		ZWrite Off 
		ZTest Always

		Pass{
			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			sampler2D _CameraDepthNormalsTexture;
			float4 _CameraDepthNormalsTexture_TexelSize;

			float4 _OutlineColor;
			float _Weight;

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v) {
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			void Compare(inout float depthOutline, inout float normalOutline, float baseDepth, float3 baseNormal, float2 uv, float2 offset){
				//read neighbor pixel
				float4 neighborDepthnormal = tex2D(_CameraDepthNormalsTexture, uv + _CameraDepthNormalsTexture_TexelSize.xy * offset);
				float3 neighborNormal;
				float neighborDepth;
				DecodeDepthNormal(neighborDepthnormal, neighborDepth, neighborNormal);
				neighborDepth = neighborDepth * _ProjectionParams.z;

				float depthDifference = baseDepth - neighborDepth;
				depthOutline += depthDifference;

				float3 normalDifference = baseNormal - neighborNormal;
				normalDifference = normalDifference.r + normalDifference.g + normalDifference.b;
				normalOutline += normalDifference;
			}

			fixed4 frag(v2f i) : SV_TARGET{
				float4 depthnormal = tex2D(_CameraDepthNormalsTexture, i.uv);

				float3 normal;
				float depth;
				DecodeDepthNormal(depthnormal, depth, normal);
				depth *= _ProjectionParams.z;

				float depthDifference = 0, normalDifference = 0;
				Compare(depthDifference, normalDifference, depth, normal, i.uv, float2(1, 0));
				Compare(depthDifference, normalDifference, depth, normal, i.uv, float2(0, 1));
				Compare(depthDifference, normalDifference, depth, normal, i.uv, float2(0, -1));
				Compare(depthDifference, normalDifference, depth, normal, i.uv, float2(-1, 0));

				depthDifference = saturate(depthDifference);
				normalDifference = saturate(normalDifference);

				float outline = normalDifference * (1 + _Weight) + depthDifference;
				float4 sourceColor = tex2D(_MainTex, i.uv);
				return lerp(sourceColor, _OutlineColor, outline);
			}
			ENDCG
		}
	}
}