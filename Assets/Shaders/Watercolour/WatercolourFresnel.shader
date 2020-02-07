Shader "Custom/WatercolourFresnel" 
{
    Properties 
    {
        _Color ("Tint Color 1", Color) = (1,1,1,1)
        _Color2 ("Tint Color 2", Color) = (1,1,1,1)
        _InkCol ("Ink Color", Color) = (1,1,1,1)
        
        _BlotchTex ("Blotches (RGB)", 2D) = "white" {}
        _DetailTex ("Detail (RGB)", 2D) = "white" {}
        _PaperTex ("Paper (RGB)", 2D) = "white" {}
        _RampTex ("Ramp (RGB)", 2D) = "white" {}
        
        _TintScale ("Tint Scale", Range(2,8)) = 4
        _PaperStrength ("Paper Strength", Range(0,1)) = 1
        _BlotchMulti ("Blotch Multiply", Range(0,8)) = 4
        _BlotchSub ("Blotch Subtract", Range(0,8)) = 2
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        
        [PowerSlider(4)] _FresnelExponent ("Fresnel Exponent", Range(.25, 4)) = 1
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _BlotchTex;
        sampler2D _DetailTex;
        sampler2D _PaperTex;
        sampler2D _RampTex;
        half _Glossiness;
        half _Metallic;
        half _BlotchMulti;
        half _BlotchSub;
        half _TintScale;
        half _PaperStrength;
        fixed4 _Color;
        fixed4 _Color2;
        fixed4 _InkCol;
        half _FresnelExponent;

        struct Input 
        {
            float2 uv_BlotchTex;
            float2 uv_DetailTex;
            float2 uv_PaperTex;
            float3 worldNormal;
            float3 viewDir;
        };

        fixed4 screen (fixed4 colA, fixed4 colB)
        {
            fixed4 white = fixed4(1,1,1,1);
            return white - (white-colA) * (white-colB);
        }
        fixed4 softlight (fixed4 colA, fixed4 colB)
        {
            fixed4 white = fixed4(1,1,1,1);
            return (white - 2 * colB) * pow(colA, 2) + 2 * colB * colA;
        }
        void surf (Input IN, inout SurfaceOutputStandard o) 
        {
            fixed c = tex2D (_BlotchTex, IN.uv_BlotchTex).r;
            c *= _BlotchMulti;
            c -= _BlotchSub;			
            c *= tex2D (_DetailTex, IN.uv_DetailTex).r;			
            c = tex2D (_RampTex, half2(c, 0)).r;
            c = saturate(c);
            
            fixed4 tint = tex2D (_BlotchTex, IN.uv_BlotchTex / _TintScale);	
            tint = lerp(_Color, _Color2, tint.r);
            
            fixed4 ink = screen(_InkCol, fixed4(c,c,c,1));

            float3 fresnel = dot(IN.worldNormal, IN.viewDir);
            fresnel = saturate(1 - fresnel);
            // fresnel = pow(fresnel, _FresnelExponent);

            // get dir and apply ramp texture along gradient
            
            o.Albedo = lerp(ink * tint, softlight(tex2D (_PaperTex, IN.uv_PaperTex), ink * tint), _PaperStrength) * fresnel;
            
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1;
        }
        
        
        ENDCG
    } 
    FallBack "Diffuse"
}
