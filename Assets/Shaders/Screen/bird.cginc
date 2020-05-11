sampler2D _BirdMask;
sampler2D _BirdBackground;

int CalculateBird(inout float4 output, float2 uv, float mask) {
	float4 birdMask =  tex2D(_BirdMask, uv);
	// output = birdMask;
	// return 1;
	
	if(birdMask.r > 0 || birdMask.g > 0 || birdMask.b > 0) {
		// float4 birdColour = float4(tex2D(_BirdBackground, uv + float2(_Time.x, _Time.x)).rgb, birdMask.b);
		// #if MASK
		// 	if(mask > .5)  birdColour.xyz = 1 - birdColour.xyz;
		// #endif
		// output = birdColour;
		// output = 1;
		return 1;
	}
	return 0;
}