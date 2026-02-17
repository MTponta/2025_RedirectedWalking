Shader "Custom/EyeRightBottomRedFilter"
{
    Properties
    {
        _MainTex ("Base (Rendered Scene)", 2D) = "white" {}
        _RedFilterColor ("Red Filter Color", Color) = (1,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _RedFilterColor;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 baseCol = tex2D(_MainTex, uv);

                // 右目かどうかチェック
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                int eye = unity_StereoEyeIndex;  // 0 = 左目, 1 = 右目

                // 条件：目が右目 AND uv.y <= 0.5（画面の下半分）
                if (eye == 1 && uv.y <= 0.5)
                {
                    // 赤フィルタをかける
                    // ここでは baseCol を赤に寄せる補正をかける例
                    // 0.5 はフィルタの強さ。調整可
                    baseCol.rgb = lerp(baseCol.rgb, _RedFilterColor.rgb, 0.5);
                }

                return baseCol;
            }
            ENDCG
        }
    }
}