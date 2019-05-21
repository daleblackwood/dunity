Shader "Dwil/WorldCurver" 
{
    Properties 
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        uniform float3 _CurveOrigin;
        uniform half _CurveScale;
        uniform float _CurveStrength;
        uniform float _CurveMax;

        sampler2D _MainTex;
        fixed4 _Color;

        struct Input 
        {
            float2 uv_MainTex;
        };

        float4 Bend(float4 v)
        {
            float4 wpos = mul (unity_ObjectToWorld, v);

            float zDist = max(0, _CurveOrigin.z - wpos.z);// - _ZFlatMargin);
            
            float zOff = zDist * zDist * 0.5;//_CurveStrength;


            // cam clamp
            //zOff *= saturate((_CurveOrigin.z - wpos.z + _ZAheadDist) * _ZAheadStrength * 0.01);

            // dampy
            zOff *= saturate(wpos.y * 0.05);

            //zOff = min(10.0, zOff);

            float offset = zOff * _CurveScale * 0.00001;
            offset = min(10, offset);
            //offset = min(_CurveMax, offset);

            wpos.z += offset;
            wpos.y += offset * 5;
            wpos = mul (unity_WorldToObject, wpos);
            
            return wpos;
        }

        void vert (inout appdata_full v) 
        {
            float4 vpos = Bend(v.vertex);   
            
            v.vertex = vpos;
        }

        void surf (Input IN, inout SurfaceOutput o) 
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }

    Fallback "Legacy Shaders/VertexLit"
}
