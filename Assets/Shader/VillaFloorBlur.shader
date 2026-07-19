Shader "ProjectNeko/VillaFloorBlur"
{
    Properties
    {
        _BlurRadius ("Blur Radius", Range(0, 12)) = 5
        _FocusBottom ("Focus Bottom", Float) = 0
        _FocusTop ("Focus Top", Float) = 1
        _Feather ("Feather", Range(0, 0.1)) = 0.02
        _EffectStrength ("Effect Strength", Range(0, 1)) = 0

        _DarkTint ("Dark Tint", Color) = (0, 0, 0, 1)
        _DarkOpacity ("Dark Opacity", Range(0, 1)) = 0.75
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "VillaFloorBlur"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _BlurRadius;
            float _FocusBottom;
            float _FocusTop;
            float _Feather;
            float _EffectStrength;

            half4 _DarkTint;
            float _DarkOpacity;

            half4 SampleScreen(float2 uv)
            {
                return SAMPLE_TEXTURE2D_X(
                    _BlitTexture,
                    sampler_LinearClamp,
                    saturate(uv)
                );
            }

            half4 GetBlurredColor(float2 uv)
            {
                float2 offset = _BlitTexture_TexelSize.xy * _BlurRadius;

                half4 color = 0;

                color += SampleScreen(uv + float2(-offset.x, -offset.y));
                color += SampleScreen(uv + float2(0,         -offset.y));
                color += SampleScreen(uv + float2(offset.x,  -offset.y));

                color += SampleScreen(uv + float2(-offset.x, 0));
                color += SampleScreen(uv);
                color += SampleScreen(uv + float2(offset.x, 0));

                color += SampleScreen(uv + float2(-offset.x, offset.y));
                color += SampleScreen(uv + float2(0,          offset.y));
                color += SampleScreen(uv + float2(offset.x,   offset.y));

                return color / 9.0;
            }

            half4 Frag(Varyings input) : SV_Target0
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord.xy;

                half4 originalColor = SampleScreen(uv);
                half4 blurredColor = GetBlurredColor(uv);

                float belowFloor = 1.0 - smoothstep(
                    _FocusBottom - _Feather,
                    _FocusBottom + _Feather,
                    uv.y
                );

                float aboveFloor = smoothstep(
                    _FocusTop - _Feather,
                    _FocusTop + _Feather,
                    uv.y
                );

                float regionMask = max(belowFloor, aboveFloor);

                // 블러된 화면에 검은색을 반투명하게 섞는다.
                half4 darkBlurredColor = blurredColor;

                darkBlurredColor.rgb = lerp(
                    blurredColor.rgb,
                    _DarkTint.rgb,
                    _DarkOpacity
                );

                // Effect Strength는 전체 효과의 켜기/끄기 역할도 한다.
                float effectMask = regionMask * _EffectStrength;

                return lerp(
                    originalColor,
                    darkBlurredColor,
                    effectMask
                );
            }

            ENDHLSL
        }
    }
}