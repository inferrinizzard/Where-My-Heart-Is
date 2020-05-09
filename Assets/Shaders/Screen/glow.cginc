sampler2D _GlowMap;
float4 _GlowMap_TexelSize;
float _Intensity;
float _Radius;

int CalculateGlow(inout float4 output, float2 uv, float mask) {
	float4 glow = tex2D(_GlowMap, uv);
	// return glow;
	if(glow.a == 0) {
		//split texel size into smaller words
		float TX_x = _GlowMap_TexelSize.x;
		float TX_y = _GlowMap_TexelSize.y;
		
		//and a final intensity that increments based on surrounding intensities.
		float4 ColorIntensityInRadius = float4(0, 0, 0, 0);
		
		for(int k = 0; k < _Radius; k++) {
			for(int j = 0; j < _Radius; j++) {
				//increase our output color by the pixels in the area
				ColorIntensityInRadius += tex2D(_GlowMap, uv.xy + 
				float2((k - _Radius / 2) * TX_x, (j - _Radius / 2) * TX_y));
			}
		}

		#if MASK
			if(mask > .5 && ColorIntensityInRadius.a > 0) return ColorIntensityInRadius * _Intensity; 
		#endif

		output += ColorIntensityInRadius * _Intensity;
	}

	return 0;
}