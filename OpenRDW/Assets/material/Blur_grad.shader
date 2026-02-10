Shader "Ultimate 10+ Shaders/Blur_grad"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _BlurAmount ("Blur Amount", Range(0, 0.03)) = 0.0128
        _MaskTex ("Blur Mask (Gradient)", 2D) = "white" {}   // ← 追加
    }

    SubShader
    {
        Tags { "Queue"="Transparent" }
        Cull Back
        ZTest Always

        GrabPass { }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 position : POSITION;
                float4 screenPos : TEXCOORD0;
                float2 uvMask : TEXCOORD1; // ← マスク用
            };

            sampler2D _GrabTexture;
            sampler2D _MaskTex;         // ← 追加

            fixed4 _Color;
            half _BlurAmount;

            v2f vert(appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.screenPos = o.position;

                // マスクはスクリーンUVと同じでOK（フルスクリーン）
                o.uvMask = o.position.xy / o.position.w;
                o.uvMask = (o.uvMask + 1.0) * 0.5;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Grab UV
                half2 uv = i.screenPos.xy / i.screenPos.w;
                uv.x = (uv.x + 1) * 0.5;
                uv.y = 1.0 - (uv.y + 1) * 0.5;

                // マスク取得
                float mask = tex2D(_MaskTex, i.uvMask).r;

                // マスクでブラー量を変動（白=100%、黒=0%）
                float blur = _BlurAmount * mask;

                half4 pixel = 0;

                // --- 基本のブラーサンプル ---
                pixel += tex2D(_GrabTexture, uv + half2( 1.5 * blur,  0.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2( 1.5 * blur, -0.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2( 1.5 * blur, -1.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2( 1.5 * blur, -2.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2( 0.5 * blur,  2.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2( 0.5 * blur,  1.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2( 0.5 * blur,  0.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2( 0.5 * blur, -0.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2( 0.5 * blur, -1.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2( 0.5 * blur, -2.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2(-0.5 * blur,  2.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2(-0.5 * blur,  1.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2(-0.5 * blur,  0.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2(-0.5 * blur, -0.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2(-0.5 * blur, -1.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2(-0.5 * blur, -2.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2(-1.5 * blur,  2.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2(-1.5 * blur,  1.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2(-1.5 * blur,  0.5 * blur));
                pixel += tex2D(_GrabTexture, uv + half2(-1.5 * blur, -0.5 * blur));

				pixel += tex2D(_GrabTexture, uv);

                return (pixel / 21.0) * _Color;
            }
            ENDCG
        }
    }
}
