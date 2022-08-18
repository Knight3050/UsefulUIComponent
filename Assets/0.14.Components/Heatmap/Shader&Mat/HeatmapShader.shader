Shader "Custom/HeatmapShader"
{
    Properties{
        [hideinInspector]
        _MainTex("MainTex",2D) = "white" {}
        _Color0("Color 0",Color) = (0,0,0,0)
        _Color1("Color 1",Color) = (0,0,1,1)
        _Color2("Color 2",Color) = (0,0.9,0.2,1)
        _Color3("Color 3",Color) = (0.9,1,0.3,1)
        _Color4("Color 4",Color) = (0.9,0.7,0.1,1)
        _Color5("Color 5",Color) = (1,0,0,1)
        // _pointRange1("pointRange1",float) = 0.35
        // _pointRange2("pointRange2",float) = 0.65
        // _pointRange3("pointRange3",float) = 0.85
        // _pointRange4("pointRange4",float) = 0.95
        area_of_effec_size("Area Effect Size", Range(0,1)) = 0.25
    }

    SubShader{
        Tags{"RenderType"="Opaque"}
        Blend SrcAlpha OneMinusSrcAlpha

        Pass{
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            struct VertexInput
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color0;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float4 _Color4;
            float4 _Color5;

            float area_of_effec_size;
            // float _pointRange1;
            // float _pointRange2;
            // float _pointRange3;
            // float _pointRange4;

            VertexOutput vert(VertexInput input)
            {
                VertexOutput o;
                o.pos = UnityObjectToClipPos(input.pos);
                o.uv = TRANSFORM_TEX(input.uv,_MainTex);
                return o;
            }

            float4 colors[6];
            float pointRanges[6];

            int _HitCount;
            float _Hits[3 * 1000];

            void Init()
            {
                colors[0] = _Color0;
                colors[1] = _Color1;
                colors[2] = _Color2;
                colors[3] = _Color3;
                colors[4] = _Color4;
                colors[5] = _Color5;

                pointRanges[0] = 0;
                pointRanges[1] = 0.35;
                pointRanges[2] = 0.65;
                pointRanges[3] = 0.85;
                pointRanges[4] = 0.95;
                pointRanges[5] = 1;
            }

            float distsq(float2 a, float2 b)
            {
                float d = pow(max(0, 1 - distance(a,b) / area_of_effec_size),2);
                return d;
            }

            float4 getHeatForPixel(float weight,float4 tex)
            {
                if(weight <= pointRanges[0])
                {
                    return colors[0];
                }
                
                if(weight >= pointRanges[5])
                {
                    return colors[5];
                }

                for(int i=0; i<6; i++)
                {
                    if(weight < pointRanges[i]){
                        float dist_from_lower_point = weight - pointRanges[i-1];
                        float size_of_point_range = pointRanges[i] - pointRanges[i-1];

                        float ratio_over_lower_point = dist_from_lower_point / size_of_point_range;

                        float4 color_range = colors[i] - colors[i-1];
                        float4 color_contribution = color_range * ratio_over_lower_point;

                        float4 new_color = colors[i-1] + color_contribution;
                        return new_color;
                    }
                }
                return tex;
            }

            float4 frag(VertexOutput input):SV_TARGET
            {
                Init();
                float4 col = tex2D(_MainTex,input.uv);
                
                float2 uv = input.uv;

                uv = uv * 4 - float2(2,2);

                float totalWeight = 0;
                float4 heat;
                if(_HitCount == 0)
                {
                    heat = colors[0];
                }
                else
                {
                    for(int i=0; i<_HitCount; i++)
                    {
                        float2 work_pt = float2(_Hits[i*3], _Hits[i*3+1]);
                        float pt_Intensity = _Hits[i*3+2];

                        totalWeight +=  distsq(uv, work_pt) * pt_Intensity;
                    }

                    heat = getHeatForPixel(totalWeight, colors[0]);
                }
                
                return heat;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
