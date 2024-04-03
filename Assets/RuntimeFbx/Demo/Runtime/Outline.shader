Shader "Hidden/RuntimeFBX/Outline"
{
	Properties
	{
		_Color("Color", Color) = (0, 0, 0, 1)
		_Scale("Scale", Range(1,2)) = 1.2
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }
		Pass
        {
            Tags {"LightMode" = "Always"}
	        ZWrite Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float _Scale;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex * _Scale);
                return o;
            }

            float4 frag (v2f i) : COLOR
            {
                return _Color;
            }
            ENDHLSL
        }

        CGPROGRAM
        #pragma surface surf Lambert

        struct Input
        {
            float4 color : COLOR;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = IN.color;
        }

        ENDCG
            

	}
	Fallback "Diffuse"
}
