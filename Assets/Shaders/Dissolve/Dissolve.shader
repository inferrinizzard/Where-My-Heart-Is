Shader "Objects/Dissolve"
{
	Properties
	{
		// [HDR] _Emission ("Emission", color) = (0,0,0)

		[Header(Dissolve)]
		_DissolveTex ("Dissolve Map", 2D) = "white" {}
		_Dissolve ("Dissolve Amount", Range(0,1)) = 0.5

		// [Header(Glow)]
		// [HDR]_GlowColor("Color", Color) = (1, 1, 1, 1)
		// _GlowRange("Range", Range(0, .3)) = 0.1
		// _GlowFalloff("Falloff", Range(0.001, .3)) = 0.1
	}
	SubShader
	{
		Tags { "Dissolve"="True" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _DissolveTex;

		struct Input
		{
			float2 uv_DissolveTex;
			float3 worldPos;
		};

		// half3 _Emission;
		fixed _Dissolve;

		// float3 _GlowColor;
		// float _GlowRange;
		// float _GlowFalloff;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float camDist = distance(IN.worldPos, _WorldSpaceCameraPos);
			float dissolve = tex2D(_DissolveTex, IN.uv_DissolveTex).r;
			dissolve = dissolve * 0.999;
			// float isVisible = dissolve - _Dissolve;
			float isVisible = dissolve - exp(-camDist);
			clip(isVisible);

			// // // // float isGlowing = smoothstep(_GlowRange + _GlowFalloff, _GlowRange, isVisible);
			// // float3 glow = isGlowing * _GlowColor;

			// fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			// o.Albedo = c.rgb;
			// o.Alpha = c.a;
			// o.Emission = _Emission + glow;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
