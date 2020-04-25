Shader "Heart/Wave"
{
	//show values to edit in inspector
	Properties {
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		[Header(Wave)]
		_WaveDistance ("Distance from player", float) = 10
		_WaveTrail ("Length of the trail", Range(0,5)) = 1
		_WaveColour ("Colour", Color) = (1, 0, 0, 1)
	}

	SubShader {
		// markers that specify that we don't need culling 
		// or comparing/writing to the depth buffer
		Cull Off ZWrite Off ZTest Always

		Pass {
			CGPROGRAM
			//include useful shader functions
			#include "UnityCG.cginc"

			//define vertex and fragment shader
			#pragma vertex vert_img
			#pragma fragment frag

			//the rendered screen so far
			sampler2D _MainTex;

			//the depth texture
			sampler2D _CameraDepthTexture;

			//variables to control the wave
			float _WaveDistance;
			float _WaveTrail;
			float4 _WaveColour;

			//the fragment shader
			fixed4 frag(v2f_img i) : SV_TARGET{
				float depth = tex2D(_CameraDepthTexture, i.uv).r;
				depth = Linear01Depth(depth);
				depth = depth * _ProjectionParams.z;

				fixed4 source = tex2D(_MainTex, i.uv);
				if(depth >= _ProjectionParams.z) return source;

				//calculate wave
				float waveFront = step(depth, _WaveDistance);
				float waveTrail = smoothstep(_WaveDistance - _WaveTrail, _WaveDistance, depth);
				float wave = waveFront * waveTrail;

				//mix wave into source color
				fixed4 col = lerp(source, _WaveColour, wave);

				return col;
			}
			ENDCG
		}
	}
}