Shader "Custom/AdvancedCRTDisplay"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}  // 主纹理
        _ScanlineStrength ("Scanline Strength", Range(0, 1)) = 0.5  // 扫描线强度
        _Resolution ("Resolution", Range(100, 1080)) = 480  // 扫描线分辨率
        _Speed ("Scanline Speed", Range(0.1, 10)) = 1.0  // 扫描线动态速度
        _Distortion ("Screen Distortion", Range(0, 0.5)) = 0.1  // 屏幕弯曲强度
        _ColorOffset ("Color Offset", Range(0, 0.01)) = 0.002  // 颜色失真偏移量
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
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

            sampler2D _MainTex;
            float _ScanlineStrength;
            float _Resolution;
            float _Speed;
            float _Distortion;
            float _ColorOffset;

            // 屏幕弯曲：径向失真
            float2 ApplyDistortion(float2 uv, float strength)
            {
                float2 center = float2(0.5, 0.5); // 屏幕中心
                float2 offset = uv - center;
                float dist = length(offset);
                uv += offset * dist * strength;
                return uv;
            }

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // 计算剪辑空间顶点位置
                o.uv = v.uv; // 将 UV 坐标传递给片段着色器
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 动态扫描线偏移
                float dynamicOffset = _Time.y * _Speed;

                // 添加屏幕弯曲效果
                float2 distortedUV = ApplyDistortion(i.uv, _Distortion);

                // 颜色失真：对 RGB 通道分别偏移
                float2 uvR = distortedUV + float2(_ColorOffset, 0);
                float2 uvG = distortedUV;
                float2 uvB = distortedUV - float2(_ColorOffset, 0);

                // 分别采样 RGB 通道
                float r = tex2D(_MainTex, uvR).r;
                float g = tex2D(_MainTex, uvG).g;
                float b = tex2D(_MainTex, uvB).b;

                // 组合颜色
                fixed4 color = float4(r, g, b, 1);

                // 动态扫描线效果
                float scanline = sin((distortedUV.y + dynamicOffset) * _Resolution * 3.14159) * 0.5 + 0.5;

                // 应用扫描线强度
                color.rgb *= lerp(1.0, scanline, _ScanlineStrength);

                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
