Shader "Custom/WaveDividerBar"
{
    Properties
    {
        _GreenColor ("Green Color", Color) = (0,1,0,1)  // 左侧绿色
        _RedColor ("Red Color", Color) = (1,0,0,1)     // 右侧红色
        _SplitRatio ("Split Ratio", Range(0,1)) = 0.5  // 红绿分割比例
        _WaveAmplitude ("Wave Amplitude", Range(0,0.1)) = 0.02  // 波动幅度
        _WaveSpeed ("Wave Speed", Range(0,10)) = 1.0   // 波动速度
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _GreenColor;
            float4 _RedColor;
            float _SplitRatio;
            float _WaveAmplitude;
            float _WaveSpeed;

            // 顶点着色器
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // 片段着色器
            fixed4 frag (v2f i) : SV_Target
            {
                // 计算当前UV的横向位置
                float x = i.uv.x;

                // 添加波动效果
                float wave = _WaveAmplitude * sin(_Time.y * _WaveSpeed + i.uv.y * 10.0);

                // 动态分割线位置（受波动影响）
                float splitPosition = _SplitRatio + wave;

                // 根据位置决定颜色
                if (x < splitPosition)
                {
                    // 绿色部分
                    return _GreenColor;
                }
                else
                {
                    // 红色部分
                    return _RedColor;
                }
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
