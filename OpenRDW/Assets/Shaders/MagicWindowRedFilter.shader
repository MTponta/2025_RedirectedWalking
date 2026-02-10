Shader "Custom/MagicWindowRedFilter"
{
    Properties
    {
        _FilterColor ("Filter Color", Color) = (1, 0.4, 0.4, 1)
        _Alpha ("Alpha", Range(0.0, 1.0)) = 0.2
        _MaskTex ("Alpha Mask (White = Visible)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _CameraOpaqueTexture;
            sampler2D_float _CameraDepthTexture;
            sampler2D _MaskTex;

            fixed4 _FilterColor;
            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex    : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float2 uv        : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 右目は描画しない（VR）
                if (unity_StereoEyeIndex == 0)
                    discard;

                // 奥だけに適用
                float sceneDepth = SAMPLE_DEPTH_TEXTURE_PROJ(
                    _CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
                float sceneLinearEyeDepth = LinearEyeDepth(sceneDepth);
                float objectLinearEyeDepth = i.screenPos.w;

                if (sceneLinearEyeDepth < objectLinearEyeDepth * 0.999)
                    discard;

                // 背景色取得
                fixed4 sceneColor =
                    tex2Dproj(_CameraOpaqueTexture, UNITY_PROJ_COORD(i.screenPos));

                // 赤フィルタ（常に100%）
                fixed3 finalColor = sceneColor.rgb * _FilterColor.rgb;

                // マスクはアルファのみに使用
                float mask = tex2D(_MaskTex, i.uv).r;
                float finalAlpha = _Alpha * mask;

                // 完全透明なら描画しない（最適化）
                if (finalAlpha <= 0.001)
                    discard;

                return fixed4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
