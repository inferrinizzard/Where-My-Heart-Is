Shader "Custom/TextureTest"{
    //show values to edit in inspector
    Properties{
        _Color ("Tint", Color) = (0, 0, 0, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _RampTex ("Gradient", 2D) = "white" {}
    }

    SubShader{
        //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
        Tags{ "RenderType"="Opaque" "Queue"="Geometry"}
        LOD 200

        Pass{
            CGPROGRAM

            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _RampTex;
            float4 _MainTex_ST;
            float4 _RampTex_TexelSize;

            fixed4 _Color;

            struct appdata{
                float4 vertex : POSITION;
                float2 uv_MainTex : TEXCOORD0;
                float2 uv_RampTex : TEXCOORD1;
            };

            struct v2f{
                float4 position : SV_POSITION;
                float4 screenPos : TEXCOORD9;
                float2 uv_MainTex : TEXCOORD0;
                float2 uv_RampTex : TEXCOORD1;
            };

            v2f vert(appdata v) {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.screenPos = v.vertex;
                // o.screenPos = ComputeScreenPos(o.position);
                o.uv_MainTex = TRANSFORM_TEX(v.uv_MainTex, _MainTex);
                o.uv_RampTex = v.uv_RampTex;
                return o;
            }

            fixed2 WorldToScreenPos(fixed3 pos){
                pos = normalize(pos - _WorldSpaceCameraPos)*(_ProjectionParams.y + (_ProjectionParams.z - _ProjectionParams.y))+_WorldSpaceCameraPos;
                fixed2 uv =0;
                fixed3 toCam = mul(unity_WorldToCamera, pos);
                fixed camPosZ = toCam.z;
                fixed height = 2 * camPosZ / unity_CameraProjection._m11;
                fixed width = _ScreenParams.x / _ScreenParams.y * height;
                uv.x = (toCam.x + width / 2)/width;
                uv.y = (toCam.y + height / 2)/height;
                return uv;
            }

            fixed4 frag(v2f i) : SV_TARGET{
                // float d = length(i.position.xy);

                fixed4 sat = tex2D (_RampTex, fixed2(0 * _RampTex_TexelSize.z, 0));

                fixed4 col = tex2D(_MainTex, i.uv_MainTex);
                col *= _Color;
                // return sat;
                return fixed4(WorldToScreenPos(i.position.xyz),0,1);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}

// void surf (Input IN, inout SurfaceOutputStandard o)
// {
    //     // Albedo comes from a texture tinted by color
    //     fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
    //     fixed4 sat = tex2D (_RampTex, fixed2(0 * _RampTex_TexelSize.z, 0));
    //     o.Albedo = sat.rgb;
    //     // Metallic and smoothness come from slider variables
    //     // o.Metallic = 0;
    //     // o.Smoothness = 0;
    //     o.Alpha = sat.a;

    //     // float3 fresnel = dot(IN.worldNormal, IN.viewDir);
    //     // fresnel = saturate(1 - fresnel);
    //     // fresnel = pow(fresnel, _FresnelExponent);
    //     // fresnel *= _FresnelColour;
    //     // o.Emission = _Emission + fresnel;
// }
