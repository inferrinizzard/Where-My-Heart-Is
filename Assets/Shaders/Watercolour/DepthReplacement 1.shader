Shader "Custom/DepthReplacement"
{
    Properties
    {
		_ColorX("Color palette X", Range(0, 0.99)) = 0
		_ColorY("Color palette Y", Range(0, 0.99)) = 0
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
		//#pragma vertex vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _CameraDepthTexture;
		float _ColorX;
		float _ColorY;

        struct Input
        {
            float2 uv_MainTex;
			float4 vertex : SV_POSITION;
			float4 screenPos;
        };

        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		/*void vert(inout appdata_full v, out Input o)
		{

		}*/

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			o.Emission.rg = float2(_ColorX, _ColorY);
			//o.Alpha = IN.screenPos.z / IN.vertex.w;
			o.Alpha = 1;
			//o.Alpha = IN.screenPos.z;
			o.Emission.b = IN.screenPos.z / (IN.screenPos.w * 0.02);
			//o.Alpha = IN.screenPos.z / IN.screenPos.w;
			//o.Emission = float3(o.Alpha, o.Alpha, o.Alpha);
			//o.Emission = float3(IN.vertex.xyz);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
