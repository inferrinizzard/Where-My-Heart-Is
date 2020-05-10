sampler2D _GlowMap;
float4 _GlowMap_TexelSize;
float _GlowIntensity;
// float _Radius;

int CalculateGlow(inout float4 output, float2 uv, float mask) {
	float4 glow = tex2D(_GlowMap, uv);

	int heartOnly = 0;

	if(glow.a == 0) {
		float TX_x = _GlowMap_TexelSize.x, TX_y = _GlowMap_TexelSize.y;
		float4 sumColour = float4(0, 0, 0, 0);
		
		int r = 5;
		for(int k = 0; k < r; k++) {
			for(int j = 0; j < r; j++) {
				float4 neighbourColour = tex2D(_GlowMap, uv + float2((k - r / 2) * TX_x, (j - r / 2) * TX_y));
				if(!heartOnly && neighbourColour.a > 0 && neighbourColour.a < 1) heartOnly = 1;
				sumColour += neighbourColour;
			}
		}

		float4 glowOutput = sumColour * _GlowIntensity;
		#if MASK
			if(sumColour.a > 0) {
				if(mask > .5) {
					if(!heartOnly) glowOutput = float4(1 - glowOutput.xyz, glowOutput.a);
				}
				else if(heartOnly) glowOutput = 0;
			}
		#endif

		// #if MASK
		// 	if(mask > .5 && sumColour.a > 0) {
			// 		output = sumColour * _GlowIntensity;
			// 		return 1;
		// 	}
		// #endif

		output += glowOutput;
	}

	return 0;
}