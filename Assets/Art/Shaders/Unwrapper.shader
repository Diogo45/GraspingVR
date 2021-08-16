Shader "Aubergine/Unwrap" {
	SubShader{
	Pass {
	Tags { "RenderType" = "Opaque" }
	Lighting Off Fog { Mode Off }
	LOD 200

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"
		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};


		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		v2f vert(appdata v)
		{
			v2f o;
			float4 sPos = v.vertex;
			sPos.xy = v.uv.xy * 2.0 - 1.0;
			sPos.z = 0;
			o.pos = mul(UNITY_MATRIX_IT_MV, sPos);
			o.uv = v.uv.xy;
			return o;
		}

		fixed4 frag(v2f i) : COLOR {
			return 1;
		}
		ENDCG
		}
	}
		FallBack Off
}