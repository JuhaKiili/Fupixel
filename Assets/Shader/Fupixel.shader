Shader "Fupixel" {
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Always
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color	: COLOR;
			};

			struct v2f
			{
				float4 vertex        : POSITION;
				float2 texcoord      : TEXCOORD0;
				float4 color		 : COLOR;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.texcoord = IN.texcoord;
				float scrX = _ScreenParams.x;
				float scrY = _ScreenParams.y;

			#ifdef UNITY_HALF_TEXEL_OFFSET
				float hpcOX = -0.5;
				float hpcOY = 0.5;
			#else
				float hpcOX = 0;
				float hpcOY = 0;
			#endif

				float pos = floor((IN.texcoord.x * 2.0 - 1.0) * scrX) + hpcOX;
				OUT.vertex.x = pos / scrX;
				pos = floor((IN.texcoord.y * 2.0 - 1.0) * scrY) + hpcOY;
				OUT.vertex.y = pos / scrY;
				OUT.vertex.z = 0.0;
				OUT.vertex.w = 1.0;
				OUT.color = IN.color;
				return OUT;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				return tex2D( _MainTex, IN.texcoord) * IN.color;
			}
		ENDCG
		}
	}
	FallBack "Diffuse"
}