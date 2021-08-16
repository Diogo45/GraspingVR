Shader "Tutorial/046_Partial_Derivatives/testing"{
	//show values to edit in inspector
	Properties{
		//_Factor("Factor", Range(0, 100)) = 1
		_MainTex("Texture", 2D) = "white"{}
		_Delta("Delta", Range(0.0001,0.001)) = 0 // sliders
	}

		SubShader{
			//the material is completely non-transparent and is rendered at the same time as the other opaque geometry
			Tags{ "RenderType" = "Opaque" "Queue" = "Geometry"}

			Cull Off

			Pass{
				CGPROGRAM

				//include useful shader functions
				#include "UnityCG.cginc"

				//define vertex and fragment shader
				#pragma vertex vert
				#pragma fragment frag

				float _Factor;
				sampler2D _MainTex;
				float _Delta;
				//Tiling: to archive that, we add a new 4 dimensional float to our global scope and call it TextureName_ST, TextureName being the name of the texture you’re tiling
				float4 _MainTex_ST;
				//the object data that's put into the vertex shader
				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float4 tangent : TANGENT;
					float4 normal : NORMAL;
				};

				//the data that's used to generate fragments and can be read by the fragment shader
				struct v2f {
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 tangent : TANGENT;
					float4 normal : NORMAL;
				};

				//the vertex shader
				v2f vert(appdata v) {
					v2f o;
					//The UnityObjectToClipPos function is inside the UnityCG.cginc file and allows us to not worry about matrix multiplication for now
					//v.vertex = float4(v.uv.xy, 0.0, 1.0);
					//o.position = mul(UNITY_MATRIX_P, v.vertex);
					o.position = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					//o.uv = v.uv;
					o.tangent = v.tangent;
					o.normal = v.normal;
					return o;
				}

				//the fragment shader
				fixed4 frag(v2f i) : SV_TARGET{
					
					float3 bitangent = cross(i.tangent.xyz, i.normal.xyz);
					float3x3 tangent2object = { i.tangent.x , bitangent.x, i.normal.x, i.tangent.y , bitangent.y, i.normal.y, i.tangent.z, bitangent.z, i.normal.z };

					float2 offset = float2(0, _Delta);

					float3 XY = mul(tangent2object, (tex2D(_MainTex, i.uv)));
					float3 ofX = mul(tangent2object, (tex2D(_MainTex, i.uv + offset.xy)));
					float3 ofY = mul(tangent2object, (tex2D(_MainTex, i.uv + offset.yx)));

					//float derivativeX = ddx(tex2D(_MainTex, i.uv).x);
					float derivativeX = (ofX - XY).x / _Delta;
					//float derivativeY = ddy(tex2D(_MainTex, i.uv).y);
					float derivativeY = (ofY - XY).y / _Delta;

					return fixed4(derivativeX, derivativeY, 0, 1);
					//return fixed4(ofX.xy, 0, 1);
				}

			ENDCG
			}
		}
}
