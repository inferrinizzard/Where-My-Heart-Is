Shader "Outline/Main"
{
	Properties {
		_MainTex("Main Texture",2D) = "black" {}
		_SceneTex("Scene Texture",2D) = "black" {}

		_Kernel("Gauss Kernel",Vector) = (0,0,0,0)
		_KernelWidth("Gauss Kernel",Float) = 1
	}
	SubShader {
		Pass {
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			float Kernel[21];
			float _KernelWidth;
			sampler2D _MainTex;    
			sampler2D _SceneTex;
			float2 _MainTex_TexelSize;

			// struct v2f {
				// 	float4 pos : SV_POSITION;
				// 	float2 uv : TEXCOORD0;
			// };

			// v2f vert (appdata_base v) {
				// 	v2f o;
				// 	o.pos = UnityObjectToClipPos(v.vertex);
				// 	o.uv = o.pos.xy / 2 + 0.5;
				// 	return o;
			// }
			
			float4 frag(v2f_img i) : COLOR {
				int NumberOfIterations = _KernelWidth;
				
				float TX_x = _MainTex_TexelSize.x;
				float TX_y = _MainTex_TexelSize.y;
				float ColorIntensityInRadius = 0;

				//for every iteration we need to do horizontally
				for(int k = 0; k<NumberOfIterations; k += 1)
				ColorIntensityInRadius += Kernel[k] * tex2D(_MainTex, float2(i.uv.x+(k-NumberOfIterations/2)*TX_x, i.uv.y)).r;
				return ColorIntensityInRadius;
			}
			ENDCG
			
		}

		GrabPass{}
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			float Kernel[21];
			float _KernelWidth;       
			sampler2D _MainTex;    
			sampler2D _SceneTex;

			sampler2D _GrabTexture;
			float2 _GrabTexture_TexelSize;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = o.pos.xy / 2 + 0.5;
				
				return o;
			}

			float4 frag(v2f i) : COLOR {
				float TX_x = _GrabTexture_TexelSize.x;
				float TX_y = _GrabTexture_TexelSize.y;

				//if something already exists underneath the fragment, draw the scene instead.
				if(tex2D(_MainTex, i.uv.xy).r > 0) { return tex2D(_SceneTex, i.uv.xy); }

				int NumberOfIterations = _KernelWidth;
				float4 ColorIntensityInRadius = 0;

				for(int k = 0; k < NumberOfIterations; k += 1) {
					ColorIntensityInRadius += Kernel[k] * tex2D(_GrabTexture, float2(i.uv.x, 1-i.uv.y+(k-NumberOfIterations/2)*TX_y));
				}

				//output the scene's color, plus our outline strength in teal.
				half4 color = tex2D(_SceneTex, i.uv.xy) + ColorIntensityInRadius * half4(0,1,1,1);
				return color;
			}
			
			ENDCG
			
		}
	}
}