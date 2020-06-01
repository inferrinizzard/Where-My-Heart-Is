float _FogDistance;
float _FogExponent;
float4 _FogColor;
sampler2D _DepthColor;

int CalculateFog(inout float4 output, float2 uv, float mask) {
	float fogStrength = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
	fogStrength = Linear01Depth(fogStrength) * _ProjectionParams.z / _FogDistance;
	// float test = tex2D(_DepthColor, uv);

	// fogStrength = fogStrength * _FogExponent;
	fogStrength = saturate(pow(fogStrength, _FogExponent));
	// output = fogStrength;
	// return 1;
	// output = lerp(output, _FogColor, fogStrength);
	output = (1 - fogStrength) * output + fogStrength * _FogColor;

	return 0;
}