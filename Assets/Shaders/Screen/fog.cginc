float _FogExponent;
float4 _FogColor;

int CalculateFog(inout float4 output, float2 uv, float mask) {
	float fogStrength = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
	fogStrength = 1 - fogStrength;
	fogStrength = saturate(pow(fogStrength, _FogExponent));
	output = (1 - fogStrength) * output + fogStrength * _FogColor;

	return 0;
}