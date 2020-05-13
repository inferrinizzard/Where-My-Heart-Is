Shader "Custom/NewWatercolor"
{
	Properties
	{
		_ColorX ("Color palette X", Range(0, 0.99)) = 0
		_ColorY ("Color palette Y", Range(0, 0.99)) = 0
		_WatercolorStrength ("Watercolor Strength", Range(0, 5)) = 0.5
		_Radius ("Radius", Range(0, 0.5)) = 0.1
		_Palette("Palette", 2D) = "white" {}
		_Paper("Paper", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_DistanceMultiplier("Distance Multiplier", float) = 1
		_NoiseScale("Noise Scale", float) = 1
		_NoiseScaleRatio("Noise Scale Ratio", float) = 1
		_PaperStrength("Paper Strength", float) = 1
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
			#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			static const float4x2 dirs = { 1, 1, -1, 1, 1, -1, -1, -1};
			static const float TAU = 6.28318530718;
			static const float PI = 3.14159265;
			static const float HALFPI = 1.57079632679;

			//static const float4x2 dirs = { 0, 1, 1, 0, 0, -1, -1, 0, 0, 1, 1, 0, 0, -1, -1, 0 };

			sampler2D _DepthColor;
			sampler2D _Noise;
			sampler2D _Palette;
			sampler2D _Paper;
			float _ColorX;
			float _ColorY;
			float _WatercolorStrength;
			float _ScreenXToYRatio;
			float _Radius;
			float _DistanceMultiplier;
			float _NoiseScale;
			float _NoiseScaleRatio;
			float _PaperStrength;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
				float4 worldVertex : POSITION1;
				float lighting : VALUE;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
				o.uv = v.uv;

				float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;
				o.lighting = dot(viewDir, v.normal);

				return o;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float2 screenCoords = IN.screenPos.xy / IN.screenPos.w;
				float4 depthColor = tex2D(_DepthColor, screenCoords);

				float noiseSample = tex2D(_Noise, IN.worldVertex.xy / _NoiseScale);
				noiseSample = tex2D(_Noise, float2(IN.worldVertex.z / (_NoiseScaleRatio * _NoiseScale), noiseSample));

				float TAUTimesNoise = TAU * noiseSample;
				float2 directionA = float2(cos(TAUTimesNoise), sin(TAUTimesNoise));

				float multiplier = _Radius * (noiseSample / 0.5);

				float outline = 0;
				float4 sampled;

				for (int i = 0; i < 4; i++)
				{
					sampled = tex2D(_DepthColor, screenCoords + saturate((dirs[i] * directionA) * multiplier) * 0.99);
					outline = saturate(abs(sampled.b - depthColor.b));
					/*if (abs(sampled.b - depthColor.b) > 0.01)
					{
						outline = 1;
					}*/
				}

				float watercolorIntensity = _WatercolorStrength * outline;
				float4 paint = float4(tex2D(_Palette, float2(min(_ColorX + watercolorIntensity, 1), _ColorY)).rgb, 1);
				return paint;
				float4 paper = tex2D(_Paper, IN.worldVertex.xz);
				
				return lerp(paper, paint, saturate(watercolorIntensity * 1/_PaperStrength));
			}
		ENDCG
		}
    }
}
