Shader "Outline/GlowObject"
{
	Properties {
		[PerRendererData]	_Colour ("Outline Colour", Color) = (1, 0, 0, 1)
		[MaterialToggle] _Occlude ("Occlusion on?", Int) = 1
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			// Properties
			sampler2D_float _CameraDepthTexture;
			fixed4 _Colour;
			float _Occlude;

			struct appdata {
				float4 vertex : POSITION;
				float3 texCoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float3 texCoord : TEXCOORD0;
				float linearDepth : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
			};

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.texCoord = v.texCoord;
				
				o.screenPos = ComputeScreenPos(o.pos);
				o.linearDepth = -(UnityObjectToViewPos(v.vertex).z * _ProjectionParams.w);
				return o;
			}

			float4 frag(v2f i) : COLOR {
				return _Colour;
				if(_Occlude == 0) { return _Colour; }
				// decode depth texture info
				float2 uv = i.screenPos.xy / i.screenPos.w; // normalized screen-space pos
				float camDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
				camDepth = Linear01Depth(camDepth); // converts z buffer value to depth value from 0..1

				float diff = saturate(i.linearDepth - camDepth);
				return diff < 0.001 ? _Colour : float4(0, 0, 0, 1);

				//return float4(camDepth, camDepth, camDepth, 1); // test camera depth value
				//return float4(i.linearDepth, i.linearDepth, i.linearDepth, 1); // test our depth
				//return float4(diff, diff, diff, 1);
			}

			ENDCG
		}
	}
}