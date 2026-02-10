Shader "Custom/LowerResBottom"
{
    Properties
    {
        _MainTex("Base (Rendered)", 2D) = "white" {}
        _Downscale("Downscale", Float) = 0.5
        _SrcHeight("Source Height", Float) = 1080
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Downscale;
            float _SrcHeight;

            float4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                float4 col;

                // uv.y: 0 画面下端、1 画面上端
                if (uv.y < 0.5)
                {
                    // 下半分: まず小さい解像度でサンプリング
                    // 縮小後の uv を計算
                    float2 scaledUV;

                    // 下半分の部分を 0→1 の範囲にマッピング
                    // uv.y from 0→0.5  → scaledY from 0→1
                    float normalizedY = uv.y / 0.5;

                    // 縮小されたテクスチャ座標
                    scaledUV.x = uv.x;
                    scaledUV.y = normalizedY;

                    // サンプリングポイントを縮小スケールに応じて補正
                    scaledUV = (scaledUV * _Downscale) + ( (1 - _Downscale) * 0.5 * float2(0,0) );

                    // 注意：上記の補正は例。Point フィルタならもっと単純に近似できる。
                    // 実際には RenderTexture を縮小してそこからのサンプリングをするか、
                    // mipmap／サンプリングオフセットを工夫する

                    col = tex2D(_MainTex, scaledUV);
                }
                else
                {
                    // 上半分は普通のサンプリング
                    col = tex2D(_MainTex, uv);
                }

                return col;
            }
            ENDCG
        }
    }
}