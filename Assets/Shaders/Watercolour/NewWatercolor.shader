Shader "Custom/NewWatercolor"
{
	Properties {
		[Header(Color and Palette)]
		_ColorX ("Color palette X", Range(0, 0.99)) = 0
		_ColorY ("Color palette Y", Range(0, 0.99)) = 0
		_Palette("Palette", 2D) = "white" {}

		[Header(Paper Texture)]
		_Paper("Paper", 2D) = "white" {}
		
		[Header(Noise Texture)]
		_Noise("Noise", 2D) = "white" {}
		_NoiseScale("Noise Scale", float) = 1
		_NoiseScaleRatio("Noise Scale Ratio", float) = 1
		
		[Header(Detail Modifiers)]
		_WatercolorStrength ("Watercolor Strength", Range(0, 5)) = 0.5
		_Radius ("Radius", Range(0, 0.5)) = 0.1
	}

	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 100

		ZWrite On
		Pass {
			CGPROGRAM
			// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
			#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			static const float4x2 dirs = { 1, 1, -1, 1, 1, -1, -1, -1};
			static const float TAU = 6.28318530718;

			// sampler2D _DepthColor; // sub for depth for colour bleed eventually

			float _ColorX;
			float _ColorY;
			sampler2D _Palette;

			sampler2D _Paper;
			sampler2D _CameraDepthTexture;

			sampler2D _Noise;
			float _NoiseScale;
			float _NoiseScaleRatio;

			float _WatercolorStrength;
			float _Radius;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 screenPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float4 worldVertex : POSITION1;
				float3 viewDir : POSITION3;
				float3 normal : POSITION2;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.screenPos = ComputeScreenPos(o.vertex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);

				float3 viewDir = normalize(UNITY_MATRIX_IT_MV[2].xyz);
				o.viewDir = viewDir - float3(0, viewDir.y, 0);// make it so looking up and down doesn't affect lighting
				
				return o;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float2 screenCoords = IN.screenPos.xy / IN.screenPos.w;

				// sample from previously rendered depth + color palette information
				float4 depthColor = tex2D(_CameraDepthTexture, screenCoords);

				// calculate lighting per frag
				float lighting = 1 - dot(IN.viewDir, IN.normal);

				// sample 2D noise using our 3D position
				float noiseSample = tex2D(_Noise, IN.worldVertex.xy / _NoiseScale);
				noiseSample = tex2D(_Noise, float2(IN.worldVertex.z / (_NoiseScaleRatio * _NoiseScale), noiseSample));

				// rotate the direction in which we sample
				float TAUTimesNoise = TAU * noiseSample;
				float2 directionA = float2(cos(TAUTimesNoise), sin(TAUTimesNoise));

				// precalculate all modifiers
				float2 multiplier = _Radius * (noiseSample / 0.5) * depthColor.b * directionA;
				
				float closest = 1000;

				// sample in four directions
				for (int i = 0; i < 4; i++) closest = min(tex2D(_CameraDepthTexture, saturate(screenCoords + dirs[i] * multiplier)).b, closest);

				//sample from paint and paper textures
				float4 paint = float4(tex2D(_Palette, float2(min(_ColorX, 1), _ColorY)).rgb, 1);
				float4 paper = tex2D(_Paper, IN.worldVertex.xz / 10);
				
				// the amount of paper vs paint is determined by the noise sample, the lighting, and the paper texture
				float t = noiseSample * lighting * lighting * (_WatercolorStrength + paper.r);
				t = saturate(max(0.5, t) + 0.35) - 0.15;

				return lerp(paper, paint, t);
			}
			ENDCG
		}
	}
	Fallback "Diffuse" 
}
