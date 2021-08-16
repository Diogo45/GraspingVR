// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Curvature/GeometryCurvature"{
	//show values to edit in inspector
	Properties{
		//_Factor("Factor", Range(0, 100)) = 1
		_MainTex("Texture", 2D) = "white"{}
		_Delta("Delta", Range(0.01,5)) = 0 // sliders
	}

		SubShader{
			//the material is completely non-transparent and is rendered at the same time as the other opaque geometry
			Tags { "RenderType" = "Opaque" }
			Blend SrcAlpha OneMinusSrcAlpha

			LOD 100
			Pass{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				float _Factor;
				sampler2D _MainTex;
				float _Delta;
				//Tiling: to archive that, we add a new 4 dimensional float to our global scope and call it TextureName_ST, TextureName being the name of the texture you’re tiling
				float4 _MainTex_ST;
				//the object data that's put into the vertex shader
				struct appdata {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float4 tangent : TANGENT;
					float4 normal : NORMAL;
				};

				//the data that's used to generate fragments and can be read by the fragment shader
				struct v2f {
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;

					float4 normal : NORMAL;
				};

				//the vertex shader
				v2f vert(appdata v) {
					v2f o;
					//The UnityObjectToClipPos function is inside the UnityCG.cginc file and allows us to not worry about matrix multiplication for now
					//v.vertex = float4((v.uv.xy - float2(0.5, 0.5)) * 2, 0.0, 1.0);
					//o.position = mul(UNITY_MATRIX_P, v.vertex);
					//v.vertex += float4(0,0,0,1);
					o.normal = v.normal;

					o.position = UnityObjectToClipPos(v.vertex);
					//o.position = v.vertex;
					o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
					//o.uv = v.uv;
					/*	
					float4 sPos = v.vertex;
					sPos.xy = v.texcoord.xy * 2.0 - 1.0;
					sPos.z = 0;
					o.position = sPos;
					o.uv = v.texcoord.xy;*/


					return o;
				}

				//the fragment shader
				fixed4 frag(v2f i) : SV_TARGET{

					//i.normal = mul(unity_WorldToObject, i.normal);
					//_Delta = 5;
					float derivativeX = ddx(i.normal.x) + 1 / (_Delta * 2);
					//float derivativeX = abs(ddx(i.normal.x)) / (_Delta);
					float derivativeY = ddy(i.normal.y) + 1 / (_Delta * 2);
					//float derivativeY = abs(ddy(i.normal.y)) / (_Delta);

					return fixed4(derivativeX, derivativeY, 0, 1);
					//return fixed4(i.normal.xy, 0, 1);
					//return fixed4(ofX.xy, 0, 1);
				}

			ENDCG
			}
		}
}
