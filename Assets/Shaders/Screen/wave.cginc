// #ifndef
sampler2D _CameraDepthTexture;
// #endif
float4x4 _ViewProjectionInverse;
float3 _WaveOrigin;

float _WaveDistance;
float _WaveTrail;
float4 _WaveColour;

float3 WorldPos(float depth, float2 uv) {
	float z = depth * 2.0 - 1.0;

	float4 clipSpacePosition = float4(uv * 2.0 - 1.0, z, 1.0);
	float4 viewSpacePosition = mul(unity_CameraInvProjection, clipSpacePosition);

	// Perspective division
	viewSpacePosition /= viewSpacePosition.w;

	float4 worldSpacePosition = mul(_ViewProjectionInverse, viewSpacePosition);

	return worldSpacePosition.xyz;
}

int CalculateWave(inout float4 output, float2 uv, float mask) {
	float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
	float waveDepth = Linear01Depth(rawDepth) * _ProjectionParams.z;

	float3 worldPos = WorldPos(rawDepth, uv);

	if(waveDepth < _ProjectionParams.z) {
		float waveFront = step(waveDepth, distance(worldPos, _WaveOrigin));
		float waveTrail = smoothstep(_WaveDistance - _WaveTrail, _WaveDistance, waveDepth);
		float wave = waveFront * waveTrail;

		output = lerp(output, _WaveColour, wave);
	}

	return 0;
}