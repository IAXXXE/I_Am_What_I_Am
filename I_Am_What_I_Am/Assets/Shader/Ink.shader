Shader "Custom/BrushTrail"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}  // 墨迹纹理
        _Color ("Color", Color) = (0,0,0,1)        // 墨色
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

            sampler2D _MainTex;
            float4 _Color;

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

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 采样纹理颜色
                fixed4 texColor = tex2D(_MainTex, i.uv);
                // 结合墨色
                return texColor * _Color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
