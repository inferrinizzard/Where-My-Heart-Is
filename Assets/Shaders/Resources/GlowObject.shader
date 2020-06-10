Shader "Outline/GlowObject"
{
	Properties {
		[PerRendererData]	_Color ("Outline Colour", Color) = (1, 0, 0, 1)
		[MaterialToggle] _Occlude ("Occlusion on?", Int) = 1
		[MaterialToggle] _Heart ("Only Heart World?", Int) = 0
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			// Properties
			sampler2D_float _CameraDepthTexture;
			fixed4 _Color;
			float _Occlude;
			float _Heart;

			struct v2f {
				float4 pos : SV_POSITION;
				float linearDepth : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.pos);
				o.linearDepth = -(UnityObjectToViewPos(v.vertex).z * _ProjectionParams.w);
				return o;
			}

			float4 frag(v2f i) : COLOR {
				float4 outColour = 1 - _Color;
				if(_Heart) outColour.a = 0.5;
				if(!_Occlude) return outColour;
				// decode depth texture info
				float2 uv = i.screenPos.xy / i.screenPos.w; // normalized screen-space pos
				float camDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
				camDepth = Linear01Depth(camDepth); // converts z buffer value to depth value from 0..1

				float diff = saturate(i.linearDepth - camDepth);
				return diff < 0.001 ? outColour : float4(0, 1, 1, 1);

				//return float4(camDepth, camDepth, camDepth, 1); // test camera depth value
				//return float4(i.linearDepth, i.linearDepth, i.linearDepth, 1); // test our depth
				//return float4(diff, diff, diff, 1);
			}

			ENDCG
		}
	}
}