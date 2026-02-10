Shader "Custom/SaturationThrough_Masked"
{
    Properties
    {
        _Saturation ("Saturation", Range(0, 10)) = 1.5
        _Color ("Tint", Color) = (1,1,1,1)
        _MaskTex ("Mask (White = Effect)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        GrabPass { }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _GrabTexture;
            sampler2D _MaskTex;
            float4 _MaskTex_ST;

            float _Saturation;
            float4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 grabPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                o.uv = TRANSFORM_TEX(v.uv, _MaskTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 背景色取得
                float2 grabUV = i.grabPos.xy / i.grabPos.w;
                fixed4 bg = tex2D(_GrabTexture, grabUV);

                // マスク（白=1, 黒=0）
                float mask = tex2D(_MaskTex, i.uv).r;

                // 彩度調整
                float lum = dot(bg.rgb, float3(0.299, 0.587, 0.114));
                float3 saturated = lerp(lum.xxx, bg.rgb, _Saturation);

                // マスクでブレンド
                bg.rgb = lerp(bg.rgb, saturated, mask);

                // ティント & 透明度
                bg *= _Color;

                return bg;
            }
            ENDCG
        }
    }
}
