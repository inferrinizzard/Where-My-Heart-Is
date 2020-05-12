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
		_Ramp("Ramp", 2D) = "white" {}
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
			sampler2D _Ramp;

			sampler2D _CameraDepthTexture;


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
				float3 viewDir : POSITION3;
				float3 normal : POSITION2;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.worldVertex = mul(unity_ObjectToWorld, v.vertex);
				o.uv = v.uv;

				float3 viewDir = UNITY_MATRIX_IT_MV[2].xyz;
				o.lighting = 1 - dot(normalize(viewDir), v.normal);
				o.viewDir = normalize(viewDir) - float3(0, normalize(viewDir).y, 0);
				//o.normal = mul(unity_ObjectToWorld, v.normal);
				o.normal = v.normal;
				return o;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				/*return float4(IN.viewDir, 1);
				return float4(IN.normal, 1);*/
				IN.lighting = 1 - dot(IN.viewDir, IN.normal);
				float2 screenCoords = IN.screenPos.xy / IN.screenPos.w;
				float4 depthColor = tex2D(_DepthColor, screenCoords);

				//return tex2D(_CameraDepthTexture, screenCoords);

				float noiseSample = tex2D(_Noise, IN.worldVertex.xy / _NoiseScale);
				noiseSample = tex2D(_Noise, float2(IN.worldVertex.z / (_NoiseScaleRatio * _NoiseScale), noiseSample));

				float TAUTimesNoise = TAU * noiseSample;
				float2 directionA = float2(cos(TAUTimesNoise), sin(TAUTimesNoise));

				//float multiplier = _Radius * (noiseSample / 0.5);
				float multiplier = _Radius * (noiseSample / 0.5) * depthColor.b;
				//return tex2D(_DepthColor, saturate(screenCoords + (float2(1, 1)) * _Radius) - 0.001).b;

				float outline = 0;
				float4 sampled;
				float closest = 1000;

				for (int i = 0; i < 4; i++)
				{
					
					sampled = tex2D(_DepthColor, saturate(screenCoords + (dirs[i] * directionA) * multiplier));
					closest = min(sampled.b, closest);
					//closest = min()
					//outline = saturate(abs(sampled.g - depthColor.g));
					//outline = max(outline, saturate(abs(sampled.g - depthColor.g)));
					//outline = abs(sampled.g - depthColor.g);
					//outline = max(outline, saturate(depthColor.b - sampled.b));
					/*if (abs(sampled.b - depthColor.b) > 0.01)
					{
						outline = 1;
					}*/
				}
				//return (abs(closest - depthColor.b) * IN.lighting) + tex2D(_Ramp, IN.lighting / _PaperStrength);
				//return tex2D(_Ramp, float2(outline, 0));
				float watercolorIntensity = _WatercolorStrength * outline;
				float4 paint = float4(tex2D(_Palette, float2(min(_ColorX, 1), _ColorY)).rgb, 1);// + watercolorIntensity
				//return paint;
				float4 paper = tex2D(_Paper, IN.worldVertex.xz / 10);
				//noiseSample = tex2D(_Noise, noiseSample * IN.worldVertex.xy / (_NoiseScale*2));
				//noiseSample = tex2D(_Noise, float2(noiseSample * IN.worldVertex.z / (_NoiseScaleRatio * (_NoiseScale * 2)), noiseSample));

				//return lerp(paper, paint, saturate(IN.lighting * watercolorIntensity / _PaperStrength));
				//return lerp(paper, paint, saturate(outline) * noiseSample * 2);//
				//return IN.lighting;
				//return tex2D(_Ramp, float2(1 - IN.lighting, 0)).r;
				//return IN.lighting;
				//return lerp(paper, paint, tex2D(_Ramp, float2((IN.lighting + outline / _PaperStrength), 0)).r);
				//return lerp(paper, paint, watercolorIntensity + IN.lighting.r);//IN.lighting.r
				//float t = (abs(closest - depthColor.b) * IN.lighting * IN.lighting);// + IN.lighting / _PaperStrength;
				float t = (noiseSample * IN.lighting * IN.lighting);// + IN.lighting / _PaperStrength;
				t *= _WatercolorStrength + paper.r;
				t = saturate(max(0.5, t) + 0.45) - 0.25;
				return lerp(paper, paint, t);//IN.lighting.r
				//(abs(closest - depthColor.b) * IN.lighting) + tex2D(_Ramp, IN.lighting / _PaperStrength)
				/*float2 screenCoords = IN.screenPos.xy / IN.screenPos.w;
				float4 depthColor = tex2D(_DepthColor, screenCoords);

				float noiseSample = tex2D(_Noise, IN.worldVertex.xy / _NoiseScale);
				noiseSample = tex2D(_Noise, float2(IN.worldVertex.z / (_NoiseScaleRatio * _NoiseScale), noiseSample));

				float TAUTimesNoise = TAU * noiseSample;

				float depthMultiplier = depthColor.b / _ProjectionParams.w;
				float multiplier = _Radius * depthMultiplier / 2000 * _DistanceMultiplier * (noiseSample / 0.5);
				float2 directionA = float2(cos(TAUTimesNoise), sin(TAUTimesNoise));

				float3 sampled;
				float closest = 1000;
				float totalDepth = 0;
				float diff = 0;
				for (int i = 0; i < 4; i++)
				{
					sampled = tex2D(_DepthColor, screenCoords + saturate((dirs[i] * directionA) * multiplier) * 0.99);
					totalDepth += sampled.b;
					closest = min(sampled.b, closest);
				}
				totalDepth = totalDepth / 4;
				//float edgeIntensity =
				float watercolorIntensity = _WatercolorStrength * abs(closest - depthColor.b) * abs(depthColor.b - totalDepth) * ;// + noiseSample;// *IN.screenPos.z / IN.screenPos.w;
				
				
				float4 paint = float4(tex2D(_Palette, float2(min(_ColorX + watercolorIntensity, 1), _ColorY)).rgb, 1);
				
				float4 paper = tex2D(_Paper, IN.worldVertex.xz);
				
				return lerp(paper, paint, saturate(watercolorIntensity * 1 / _PaperStrength));

				//return float4(float3(1, 1, 1) * (abs(closest - depthColor.b) * abs(totalDepth/8)), 1);*/
			}
		ENDCG
		}
    }
}
