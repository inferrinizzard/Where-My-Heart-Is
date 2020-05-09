sampler2D _BirdMask;
sampler2D _BirdBackground;

int CalculateBird(inout float4 output, float2 uv, float mask) {
	float4 birdMask =  tex2D(_BirdMask, uv);
	// if(birdMask.r > 0 || birdMask.g > 0 || birdMask.b > 0) {
		// 	#if MASK
		// 		if(mask > .5)  return 1 - float4(tex2D(_BirdBackground, uv + float2(_Time.x, _Time.x)).rgb, birdMask.b);
		// 	#endif
		// 	return float4(tex2D(_BirdBackground, uv + float2(_Time.x, _Time.x)).rgb, birdMask.b);
	// }
	return 0;
}