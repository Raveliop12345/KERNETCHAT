Shader "AnimationCraft/DitherLitVertexColor"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _Fade("Fade", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 200
        Pass
        {
            Name "ForwardLit"
            Tags{ "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : NORMAL;
                float4 color       : COLOR;
                float3 positionWS  : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _Fade;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(OUT.positionWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.color = IN.color * _BaseColor;
                return OUT;
            }

            float DitherThreshold(float2 pos)
            {
                const float4x4 bayer = float4x4(
                    0/16.0,  8/16.0,  2/16.0, 10/16.0,
                    12/16.0, 4/16.0, 14/16.0, 6/16.0,
                    3/16.0, 11/16.0, 1/16.0,  9/16.0,
                    15/16.0,7/16.0, 13/16.0, 5/16.0);
                int2 p = int2(pos) & 3;
                return bayer[p.y][p.x];
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float3 normalWS = normalize(IN.normalWS);
                Light light = GetMainLight(TransformWorldToShadowCoord(IN.positionWS));
                float NdotL = saturate(dot(normalWS, light.direction));
                float3 litColor = IN.color.rgb * (0.3 + 0.7 * NdotL * light.color.rgb);

                float d = DitherThreshold(IN.positionHCS.xy);
                if (_Fade < d) discard;

                return half4(litColor, 1);
            }
            ENDHLSL
        }
    }
}
