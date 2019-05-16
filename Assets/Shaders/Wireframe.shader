Shader "Custom/Wireframe"
{
    Properties
    {
        [PowerSlider(3.0)]
        _WireframeVal ("Width", Range(0., 0.34)) = 0.05
        _FrontColor ("Color", color) = (1., 1., 1., 1.)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
 
        Pass
        {
            Cull Back
            CGPROGRAM
			#pragma require geometry
			#pragma target 3.0
 
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            #include "UnityCG.cginc"
 
            struct v2g {
                float4 pos : SV_POSITION;
            };
 
            struct g2f {
                float4 pos : SV_POSITION;
                float3 bary : TEXCOORD0;
            };
 
            v2g vert(appdata_base v) {
                v2g o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
 
            [maxvertexcount(3)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream) {
                g2f o;
                o.pos = IN[0].pos;
                o.bary = float3(1., 0., 0.);
                triStream.Append(o);
                o.pos = IN[1].pos;
                o.bary = float3(0., 0., 1.);
                triStream.Append(o);
                o.pos = IN[2].pos;
                o.bary = float3(0., 1., 0.);
                triStream.Append(o);
            }
 
            float _WireframeVal;
            fixed4 _FrontColor;
 
            fixed4 frag(g2f i) : SV_Target {
				if(!any(bool3(i.bary.x < _WireframeVal, i.bary.y < _WireframeVal, i.bary.z < _WireframeVal)))
					discard;
 
                return _FrontColor;
            }
 
            ENDCG
        }
    }
}
