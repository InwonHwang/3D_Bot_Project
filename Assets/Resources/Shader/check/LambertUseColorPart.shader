Shader "Custom/LambertUseColorPart" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_PartsColor ("Parts Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[Toggle(_INTERPOLATED_PARTS_COLOR)] _InterpolatedPartsColor ("Interpolated Parts Color", Int) = 0
		[Toggle(_INVERT_PARTS_COLOR_MASK)] _InvertPartsColorMask ("Invert Parts Color Mask", Int) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf Lambert fullforwardshadows
		#pragma shader_feature _INTERPOLATED_PARTS_COLOR 
		#pragma shader_feature _INVERT_PARTS_COLOR_MASK

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;
		fixed4 _PartsColor;

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
#if _INVERT_PARTS_COLOR_MASK
			c.a = 1 - c.a;
#endif			

#if _INTERPOLATED_PARTS_COLOR
			o.Albedo = lerp(c.rgb, _PartsColor.rgb, c.a);
#else
			o.Albedo = c.rgb * lerp( float3(1,1,1), _PartsColor.rgb, c.a);
#endif
			o.Alpha = 1;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

