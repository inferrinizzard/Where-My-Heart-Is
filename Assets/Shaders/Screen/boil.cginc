// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles
sampler2D _CameraDepthNormalsTexture;
sampler2D _HeartDepthNormals;
float4 _CameraDepthNormalsTexture_TexelSize;
static const float4x2 dirs = { 0, 1, 1, 0, 0, -1, -1, 0 };

float4 _DepthOutlineColour;
float _NormalMult, _NormalBias;
float _DepthMult, _DepthBias;

sampler2D _HatchTex;
int _HatchSize, _HatchSpeed;
float _Distortion, _NoiseSpeed;


void Compare(inout float depthOutline, inout float normalOutline, float baseDepth, float3 baseNormal, float2 uv, float2 offset, int maskOn) {
	//read neighbor pixel
	float4 neighbour = maskOn ? tex2D(_HeartDepthNormals, uv + _CameraDepthNormalsTexture_TexelSize.xy * offset) : tex2D(_CameraDepthNormalsTexture, uv + _CameraDepthNormalsTexture_TexelSize.xy * offset);
	float3 neighborNormal;
	float neighborDepth;
	DecodeDepthNormal(neighbour, neighborDepth, neighborNormal);
	neighborDepth *= _ProjectionParams.z;

	depthOutline += baseDepth - neighborDepth;

	float3 normalDifference = baseNormal - neighborNormal;
	normalDifference = normalDifference.r + normalDifference.g + normalDifference.b;
	normalOutline += normalDifference;
}

bool rgbEquals(fixed4 a, fixed4 b, fixed epsilon = 0.1) {
	// return a.r == b.r && a.g == b.g && a.b == b.b;
	if(abs(a.r - b.r) > epsilon) return false;
	if(abs(a.g - b.g) > epsilon) return false;
	if(abs(a.b - b.b) > epsilon) return false;
	return true;
}

float2 modUV(float2 uv, float row, float col, int size) {
	row = row % size;
	col = col % size;
	return (uv / size) + float2(row/size, col/size);
}

int CalculateBoil(inout float4 output, float2 uv, float mask) {
	float texIndex = floor(_Time.y * _HatchSpeed % 9);
	float row = 1 + texIndex % (_HatchSize - 1);
	float col = floor(texIndex / _HatchSize);

	int maskOn = 0;
	// #if MASK
	// 	if(mask > .5) maskOn = 1;
	// #endif

	float4 depthNormal = maskOn ? tex2D(_HeartDepthNormals, uv) : tex2D(_CameraDepthNormalsTexture, uv);

	float3 normal;
	float boilDepth;
	DecodeDepthNormal(depthNormal, boilDepth, normal);
	boilDepth *= _ProjectionParams.z;

	float depthDifference = 0;
	float normalDifference = 0;

	for(int j = 0; j < 4; j++) Compare(depthDifference, normalDifference, boilDepth, normal, uv, dirs[j], maskOn);

	depthDifference *= _DepthMult;
	depthDifference = saturate(depthDifference);
	depthDifference = pow(depthDifference, _DepthBias);

	normalDifference *= _NormalMult;
	normalDifference = saturate(normalDifference);
	normalDifference = pow(normalDifference, _NormalBias);

	float outline = normalDifference + depthDifference * 2;
	float4 color = lerp(output, _DepthOutlineColour, outline);
	// if (outline > 0.1)
	// {
		// 	float4 hatchColor = tex2D(_HatchTex, modUV(i.uv * _HatchSize, row, col, _HatchSize));
		// 	color = lerp(color, output, 1 - hatchColor.r);// subtract back to non-outline based on brightness of hatch
		// 	color = hatchColor;
	// }

	if(!rgbEquals(output, color)) { 
		float4 preOutline = tex2D(_HatchTex, modUV(uv * _HatchSize, row, col, _HatchSize));
		preOutline = lerp(color, output, 1 - preOutline.r);
		#if MASK
			if(mask > .5) preOutline = 0;
		#endif
		output += preOutline;
	}

	return 0;
}