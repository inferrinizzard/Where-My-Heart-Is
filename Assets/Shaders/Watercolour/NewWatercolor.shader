Shader "Custom/NewWatercolor"
{
	Properties
	{
		_ColorX ("Color palette X", Range(0, 0.99)) = 0
		_ColorY ("Color palette Y", Range(0, 0.99)) = 0
		_WatercolorStrength ("Watercolor Strength", Range(0, 0.99)) = 0.5
		_Radius ("Radius", Range(0, 0.2)) = 0.1
		_Palette("Palette", 2D) = "white" {}
		_Noise("Noise", 2D) = "white" {}
		_DistanceMultiplier("Distance Multiplier", float) = 1
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
			/*static const float4x2 dirs = { 0, 1, 1, 0, 0, -1, -1, 0 };
			static const float4x2 shmirs = { 0.702, 0.702, -0.702, 0.702, 0.702, -0.702, -0.702, -0.702 };*/
			static const float TAU = 6.28318530718;
			static const float PI = 3.14159265;
			static const float HALFPI = 1.57079632679;

			//static const float4x2 dirs = { 0, 1, 1, 0, 0, -1, -1, 0, 0, 1, 1, 0, 0, -1, -1, 0 };

			sampler2D _DepthColor;
			sampler2D _Noise;
			sampler2D _Palette;
			float _ColorX;
			float _ColorY;
			float _WatercolorStrength;
			float _ScreenXToYRatio;
			float _Radius;
			float _DistanceMultiplier;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 screenPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float2 screenCoords = IN.screenPos.xy / IN.screenPos.w;
				float4 depthColor = tex2D(_DepthColor, screenCoords);

				float closest = 1000;
				//float curDepth = 0;
				float totalDepth = 0;
				float depthMultiplier = depthColor.b;
				//float noiseSample = tex2D(_Noise, IN.uv);
				//float noiseSample = tex2D(_Noise, screenCoords);
				float noiseSample = tex2D(_Noise, screenCoords);

				float TAUTimesNoise = TAU * noiseSample;

				float2 directionA = float2(cos(TAUTimesNoise), sin(TAUTimesNoise));

				float multiplier = _Radius * depthMultiplier * (noiseSample / 0.5);
				float3 sampled;
				float totalHue = 0;
				for (int i = 0; i < 4; i++)
				{
					sampled = tex2D(_DepthColor, saturate(screenCoords + (dirs[i] * directionA) * multiplier));
					totalDepth += sampled.b;
					closest = min(sampled.b, closest);
					totalHue += sampled.g;
				}
				
				float watercolorIntensity = _WatercolorStrength * (abs(closest - depthColor.b) * abs(totalDepth / 4)) + noiseSample * IN.screenPos.z / IN.screenPos.w;
				//float watercolorIntensity = _WatercolorStrength * (abs(closest - depthColor.b) + abs((totalDepth / 4) - depthColor.b));
				//watercolorIntensity += ;

				//return float4(tex2D(_Palette, float2(min(_ColorX + watercolorIntensity, 1), (_ColorY + (totalHue / 4)) / 2)).rgb, 1);
				return float4(tex2D(_Palette, float2(min(_ColorX + watercolorIntensity, 1), _ColorY)).rgb, 1);
				//return float4(float3(1, 1, 1) * (abs(closest - depthColor.b) * abs(totalDepth/8)), 1);
			}
		ENDCG
		}
    }
}
