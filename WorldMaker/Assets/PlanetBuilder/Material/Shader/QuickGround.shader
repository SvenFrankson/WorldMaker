Shader "Custom/QuickGround" {
	Properties {
		_ColorBase ("Color Base", Color) = (1,1,1,1)
		_ColorStripes ("Color Stripes", Color) = (1,1,1,1)
		_StripeSpacing ("Stripe Spacing", Int) = 2
		_StripeSize ("Stripe Size", Int) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		fixed4 _ColorBase;
		fixed4 _ColorStripes;
		int _StripeSpacing;
		int _StripeSize;

		struct Input {
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c;
			if (length(IN.worldPos) - ((int) length(IN.worldPos) / _StripeSpacing) * _StripeSpacing < _StripeSize) {
				c = _ColorStripes;
			}
			else {
				c = _ColorBase;
			}
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
