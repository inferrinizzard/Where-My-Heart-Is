﻿Shader "Watercolour/Transparent"
{
	Properties {
		_Color ("Tint Color 1", Color) = (1,1,1,1)
		_Color2 ("Tint Color 2", Color) = (1,1,1,1)
		_InkCol ("Ink Color", Color) = (1,1,1,1)
		
		_BlotchTex ("Blotches (RGB)", 2D) = "white" {}
		_DetailTex ("Detail (RGB)", 2D) = "white" {}
		_PaperTex ("Paper (RGB)", 2D) = "white" {}
		_RampTex ("Ramp (RGB)", 2D) = "white" {}
		
		_TintScale ("Tint Scale", Range(2,8)) = 4
		_PaperStrength ("Paper Strength", Range(0,1)) = 1
		_BlotchMulti ("Blotch Multiply", Range(0,16)) = 4
		_BlotchSub ("Blotch Subtract", Range(0,8)) = 2

		[PowerSlider(8)] _FresnelExponent ("Fresnel Exponent", Range(0, 4)) = 1

		_Dissolve ("Dissolve", int) = 0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "LightMode"="ForwardBase"}
		LOD 200

		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		// #pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma surface surf Standard noforwardadd
		#pragma target 3.5
		// #pragma debug

		#pragma multi_compile_local __ DISSOLVE DISSOLVE_MANUAL

		sampler2D _BlotchTex;
		sampler2D _DetailTex;
		sampler2D _PaperTex;
		sampler2D _RampTex;
		float4 _RampTex_TexelSize;
		half _BlotchMulti;
		half _BlotchSub;
		half _TintScale;
		half _PaperStrength;
		fixed4 _Color;
		fixed4 _Color2;
		fixed4 _InkCol;
		half _FresnelExponent;

		int _Dissolve;
		float3 _ViewDir;
		float _ManualDissolve;

		struct Input {
			float2 uv_BlotchTex;
			float2 uv_DetailTex;
			float2 uv_PaperTex;
			float2 uv_RampTex;
			float3 worldNormal;
			float3 worldPos;
			float3 viewDir;
			// float3 lightDir;
			// float lightAtten;
		};

		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert (inout appdata_base v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			// 	float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

			// 	float3 lightDir = float3(0, 0, 0);
			// 	float lightAtten = 0;
			// 	int lights = 4;
			
			// 	for(int i = 0; i < 4; i++) {
				// 		float3 lightPos = float3(unity_4LightPosX0[i], unity_4LightPosY0[i], unity_4LightPosZ0[i]);
				// 		if(lightPos[0] == 0 && lightPos[1] == 0 && lightPos[2] == 0) {
					// 			lights--;
					// 			continue;
				// 		}
				// 		lightDir += normalize(lightPos - worldPos);
				// 		lightAtten += (1 - unity_4LightAtten0[i]) * length(unity_LightColor[0]);
				// 		// lightAtten += length(unity_LightColor[0]);
			// 	}
			// 	o.lightDir = lightDir / lights;
			// 	o.lightAtten = lightAtten / lights;
			// 	// o.lightDir = normalize(lightDir);

			// 	// bgolus god fix
			// 	// float range = (0.005 * sqrt(1000000 - unity_4LightAtten0[0]])) / sqrt(unity_4LightAtten0[0]]);
			// 	// float attenUV = distance(lightPos, worldPos) / range;
			// 	// float atten = saturate(1.0 / (1.0 + 25.0 * attenUV*attenUV) * saturate((1 - attenUV) * 5.0));
			// 	// float atten = tex2D(_LightTextureB0, (attenUV * attenUV).xx).UNITY_ATTEN_CHANNEL;
		}

		fixed4 screen (fixed4 colA, fixed4 colB) {
			fixed4 white = fixed4(1, 1, 1, 1);
			return white - (white - colA) * (white - colB);
		}

		fixed4 softlight (fixed4 colA, fixed4 colB) {
			fixed4 white = fixed4(1, 1, 1, 1);
			return (white - 2 * colB) * pow(colA, 2) + 2 * colB * colA;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			#if DISSOLVE
				if(_Dissolve == 1) {
					float camDist = distance(IN.worldPos, _WorldSpaceCameraPos + float3(_ViewDir.x, max(0, _ViewDir.y), _ViewDir.z));
					float isVisible = tex2D(_DetailTex, IN.uv_DetailTex).r * 0.999 - exp(-camDist);
					clip(isVisible);
				}
			#elif DISSOLVE_MANUAL
				float isVisible = tex2D(_DetailTex, IN.uv_DetailTex).r * 0.999 - _ManualDissolve;
				clip(isVisible);
			#endif

			fixed c = tex2D (_BlotchTex, IN.uv_BlotchTex).r;
			c *= _BlotchMulti;
			c -= _BlotchSub;			
			c *= tex2D (_DetailTex, IN.uv_DetailTex).r;			

			// float f = (1 - dot(IN.worldNormal, IN.lightDir)) * IN.lightAtten;
			float f = dot(IN.worldNormal, IN.viewDir);
			f = pow(f, _FresnelExponent);

			c = saturate(c * .3 + f);
			c = tex2D (_RampTex, half2(1 - c, 0)).r;
			c = saturate(c);

			fixed4 tint = tex2D (_BlotchTex, IN.uv_BlotchTex / _TintScale);	
			tint = lerp(_Color, _Color2, tint.r);
			
			fixed4 ink = screen(_InkCol, fixed4(c, c, c, 1));

			o.Albedo = lerp(ink * tint, softlight(tex2D (_PaperTex, IN.uv_PaperTex), ink * tint), _PaperStrength);
			// o.Albedo = IN.lightDir * IN.lightAtten;
			// o.Albedo = dot(IN.worldNormal, IN.lightDir);
		}
		ENDCG
	}
	FallBack "Diffuse"
}