Shader "Custom/QuickGround" {
	Properties {
		_GrassColor ("Grass Color", Color) = (0,0,0,0)
		_CliffColor ("Cliff Color", Color) = (0,0,0,0)
		_SandColor ("Sand Color", Color) = (0,0,0,0)
		_CliffThreshold ("Cliff Threshold", Float) = 0.9
		_CliffOutlineSize ("Cliff Merge Size", Float) = 0.05
		_CliffOutlineColor ("Cliff Outline Color", Color) = (0,0,0,0)
		_SandThreshold ("Sand Threshold", Float) = 8
		_PlanetPosition ("Planet Position", Vector) = (0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		fixed4 _GrassColor;
		fixed4 _CliffColor;
		fixed4 _SandColor;
		fixed4 _CliffOutlineColor;
		float _CliffThreshold;
		float _CliffOutlineSize;
		float _SandThreshold;
		float3 _PlanetPosition;

		struct Input {
			float2 uv_GrassTex;
			float3 worldNormal;
			float3 worldPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = _CliffOutlineColor;
			float nUp = dot(IN.worldNormal, normalize(IN.worldPos - _PlanetPosition));
			if (nUp < _CliffThreshold - _CliffOutlineSize) {
				c = _CliffColor;
			}
			if (nUp > _CliffThreshold + _CliffOutlineSize) {
				if (length(IN.worldPos - _PlanetPosition) > _SandThreshold) {
					c = _GrassColor;
				}
				else {
					c = _SandColor;
				}
			}
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}