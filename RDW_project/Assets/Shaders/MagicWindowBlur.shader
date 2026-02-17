Shader "Custom/MagicWindowBlur_ObjectUVMasked"
{
    Properties
    {
        _BlurSize ("Blur Size", Range(0.0, 0.01)) = 0.002
        _Alpha ("Alpha", Range(0.0, 1.0)) = 0.1
        _MaskTex ("Mask (White = Effect)", 2D) = "white" {}
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
            float4 _CameraOpaqueTexture_TexelSize;

            sampler2D _MaskTex;

            float _BlurSize;
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
                // 右目は描画しない（VR用）
                #if UNITY_STEREO_INSTANCING_ENABLED
                if (unity_StereoEyeIndex == 1)
                    discard;
                #endif

                // 深度判定（このオブジェクトより奥だけ処理）
                float sceneDepth = SAMPLE_DEPTH_TEXTURE_PROJ(
                    _CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos));
                float sceneLinearEyeDepth = LinearEyeDepth(sceneDepth);
                float objectLinearEyeDepth = i.screenPos.w;

                if (sceneLinearEyeDepth < objectLinearEyeDepth * 0.999)
                    discard;

                // ★ UVマスク取得
                float mask = tex2D(_MaskTex, i.uv).r;

                // 完全に黒なら描画しない
                if (mask <= 0.001)
                    discard;

                // ブラーオフセット
                float2 offset = _BlurSize * _CameraOpaqueTexture_TexelSize.xy;

                // 5点ぼかし
                fixed4 col = tex2Dproj(_CameraOpaqueTexture, UNITY_PROJ_COORD(i.screenPos));
                col += tex2Dproj(_CameraOpaqueTexture, UNITY_PROJ_COORD(i.screenPos + float4(0, offset.y, 0, 0)));
                col += tex2Dproj(_CameraOpaqueTexture, UNITY_PROJ_COORD(i.screenPos + float4(0, -offset.y, 0, 0)));
                col += tex2Dproj(_CameraOpaqueTexture, UNITY_PROJ_COORD(i.screenPos + float4(offset.x, 0, 0, 0)));
                col += tex2Dproj(_CameraOpaqueTexture, UNITY_PROJ_COORD(i.screenPos + float4(-offset.x, 0, 0, 0)));

                fixed3 finalColor = col.rgb / 5.0;

                // ★ マスクでアルファ制御
                float finalAlpha = _Alpha * mask;

                return fixed4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
