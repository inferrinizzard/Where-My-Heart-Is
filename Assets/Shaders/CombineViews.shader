Shader "Custom/CombineViews"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_Mask ("Mask", 2D) = "white" {}
		_Sculpture("Sculpture", 2D) = "white" {}
		_Play("Play", 2D) = "white" {}
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
		}
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

		//Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

			sampler2D _MainTex;
			sampler2D _Mask;
			sampler2D _Sculpture;
			sampler2D _Play;

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 col;
				fixed4 mask = tex2D(_Mask, i.uv);
				if (mask.r > 0.5)// if the pixel on the mask is the designated true value
				{
					col = tex2D(_Sculpture, i.uv);
				}
				else
				{
					col = tex2D(_Play, i.uv);
				}
                return col;
            }
            ENDCG
        }
    }
}
