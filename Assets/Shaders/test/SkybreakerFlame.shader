Shader "Custom/SkybreakerFlame" 
{
	Properties 
	{
        _MainTex ("Noise / Detail", 2D) = "white" {}
        _GradTex ("Gradient", 2D) = "white" {}
        _Exaggeration ("Edge Exaggeration", Range(1,30)) = 10
        _Strength ("Flame Strength", Range(-2,2)) = 0
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader 
    {
   		 Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
         ZWrite Off
         Blend SrcAlpha OneMinusSrcAlpha
        Pass 
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag alpha

            #include "UnityCG.cginc"
            
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform sampler2D _GradTex;
            uniform float4 _GradTex_ST;
            uniform float _Exaggeration;
            uniform float _Strength;
            uniform float4 _Color;

            fixed4 frag(v2f_img i) : SV_Target 
            {
                float4 col = tex2D(_MainTex, i.uv*_MainTex_ST.xy + _MainTex_ST.zw);
                col += 2*tex2D(_GradTex, i.uv*_GradTex_ST.xy + _GradTex_ST.zw).r-1;
                col = _Exaggeration*(-0.5 + col + _Strength) + 0.5;
                col.a = clamp(col.r,0,1)*_Color.a;
                col.rgb = _Color.rgb;
                return col;
            }
            ENDCG
        }
    }
}
