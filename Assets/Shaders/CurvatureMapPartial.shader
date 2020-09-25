Shader "Curvature/GeometryCurvaturePartial"{
	//show values to edit in inspector
	Properties{
		//_Factor("Factor", Range(0, 100)) = 1
		_MainTex("Texture", 2D) = "white"{}
		_Delta("Delta", Range(0,1)) = 0 // sliders
		[KeywordEnum(X, Y, Combined, Test, TestX, TestY)] _Type("Type Mode", Float) = 0
		[KeywordEnum(Screen, Object)] _RenderType("Render Mode", Float) = 0
		_MagnitudeIntensify("BrighterColors", Range(0, 1000)) = 0
			_Offset("Offset", Range(0.0001,0.02)) = 0.1
	}

		SubShader{
			//the material is completely non-transparent and is rendered at the same time as the other opaque geometry
			Tags { "RenderType" = "Transparent"
			"Queue" = "Transparent" }
			Cull Off


			LOD 100
			Pass{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				#pragma multi_compile_fog
				#pragma multi_compile _TYPE_X _TYPE_Y _TYPE_COMBINED _TYPE_TEST _TYPE_TEST_X _TYPE_TEST_Y
				#pragma multi_compile _RENDERTYPE_SCREEN _RENDERTYPE_OBJECT

				#include "UnityCG.cginc"

				float _Factor;
				sampler2D _MainTex;
				float _Delta, _Offset;
				float _MagnitudeIntensify;
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
					float4 normal : NORMAL;
				};

				//the vertex shader
				v2f vert(appdata v) {
					v2f o;
					/// Texture to screen
#ifdef _RENDERTYPE_SCREEN
					v.vertex = float4((v.uv.xy - float2(0.5, 0.5)) * 2, 0.0, 1.0);
					o.position = v.vertex;
					o.uv = v.uv;
#endif
#ifdef _RENDERTYPE_OBJECT
					// Object 
					o.position = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
#endif

					o.normal = v.normal;
					return o;
				}

				//the fragment shader
				fixed4 frag(v2f i) : SV_TARGET{

					i.normal = mul(unity_WorldToObject, i.normal);
					//_Delta = 5;


					//return float4(i.uv, 0, 1);
					//float derivativeY = abs(ddy(i.normal.y)) / (_Delta);


					float3 derivX = (ddx(i.normal) /*+ float3(0.5, 0, 0)*/) / 1;

					float3 derivY = (ddy(i.normal) /*+ float3(0, 0.5, 0)*/) / 1;

					float3 deriv = (derivX + derivY);


					//return float4(i.norm2.xyz, 1);
					//return fixed4((ddx(i.normal.x) + 1)/2, (ddy(i.normal.y) + 1)/2,0,1);
					#ifdef _TYPE_X
					//return fixed4((derivX + _Offset) * _MagnitudeIntensify - _Offset, 1);
					return float4((derivX + float3(1, 1, 1)) / 2, 1);
					#endif
					#ifdef _TYPE_Y
					//return fixed4((derivY + _Offset) * _MagnitudeIntensify - _Offset, 1);
					return float4((derivY + float3(1, 1, 1)) / 2, 1);
					#endif
					#ifdef _TYPE_COMBINED
					return float4(((deriv) + float3(1, 1, 1))/2 , 1);
					#endif
#ifdef _TYPE_TEST
					float derivFX = ddx(i.normal.x);

					float derivFY = ddy(i.normal.y);

					float3 derivF = float3(0, derivFY, 0) /** _MagnitudeIntensify*/;

					return float4(((derivF)+float3(_Delta, _Delta, _Delta)) / 2, 1);
#endif
//#ifdef _TYPE_TEST_X
//					float derivFX = ddx(i.normal.x);
//
//					float derivFY = ddy(i.normal.y);
//
//					float3 derivF = float3(derivFX, 0, 0) * _MagnitudeIntensify;
//
//					return float4(((derivF)+float3(_Delta, _Delta, _Delta)) / 2, 1);
//#endif
//#ifdef _TYPE_TEST_Y
//					float derivFX = ddx(i.normal.x);
//
//					float derivFY = ddy(i.normal.y);
//
//					float3 derivF = float3(derivFX, derivFY, 0) * _MagnitudeIntensify;
//
//					return float4(((derivF)+float3(_Delta, _Delta, _Delta)) / 2, 1);
//#endif




					//return fixed4(i.normal.xyz, 1);
					//return fixed4(ofX.xyz, 1);
				}

			ENDCG
			}
		}
}
