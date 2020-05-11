// sampler2D _BirdMask;
// sampler3D _GlowMap;
sampler2D _BirdBackground;

int CalculateBird(inout float4 output, float2 uv, float mask) {
	float4 birdMask =  tex2D(_GlowMap, uv);
	// output = birdMask;
	// return 1;
	
	if(birdMask.r > 0 || birdMask.g > 0 || birdMask.b > 0) {
		// float3 birdColour = tex2D(_BirdBackground, uv + float2(_Time.x, _Time.x)).rgb;
		// #if MASK
		// 	if(mask > .5)  birdColour = 1 - birdColour;
		// #endif
		// output = float4(lerp(output, birdColour, birdMask.b), 1);
		output += float4(tex2D(_BirdBackground, uv + float2(_Time.x, _Time.x)).xyz, birdMask.b);
		return 1;
	}
	return 0;
}