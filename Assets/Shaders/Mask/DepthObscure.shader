Shader "Depth/Obscure"
{
	Properties {
	}
	SubShader {
		Tags { "RenderType" = "Opaque" "Queue"="Transparent+1" }
		LOD 100

		Pass {
			Blend Zero One
			ZWrite On
			ZTest Always
			// Cull Front
		}
	}
}