// #ifndef
sampler2D _CameraDepthTexture;
// #endif

float _WaveDistance;
float _WaveTrail;
float4 _WaveColour;

int CalculateWave(inout float4 output, float2 uv, float mask) {
	float waveDepth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv)) * _ProjectionParams.z;

	if(waveDepth < _ProjectionParams.z) {
		float waveFront = step(waveDepth, _WaveDistance);
		float waveTrail = smoothstep(_WaveDistance - _WaveTrail, _WaveDistance, waveDepth);
		float wave = waveFront * waveTrail;

		output = lerp(output, _WaveColour, wave);
	}

	return 0;
}