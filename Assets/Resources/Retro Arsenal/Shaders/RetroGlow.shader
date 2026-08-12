// Made with Amplify Shader Editor v1.9.8.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/RetroArsenal/BasicGlowURP"
{
    Properties
    {
        _Tint("Tint", Color) = (1,1,1,1)
        _ExtraGlow("Extra Glow", Range( 0 , 15)) = 0
        _TextureSample("Texture Sample", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "ForwardLit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float4 color        : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 vertexColor  : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Tint;
                float _ExtraGlow;
                SAMPLER(sampler_TextureSample);
                float4 _TextureSample_ST;
            CBUFFER_END
            TEXTURE2D(_TextureSample);

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _TextureSample);
                output.vertexColor = input.color;
                return output;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_TextureSample, sampler_TextureSample, i.uv);
                half3 col = tex.rgb * i.vertexColor.rgb * _Tint.rgb * _ExtraGlow;
                half alpha = tex.a * i.vertexColor.a;

                half4 final;
                final.rgb = col;
                final.a = alpha;
                return final;
            }
            ENDHLSL
        }

        // URP “ı”∞Õ∂…‰Pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };
            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float alpha : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                SAMPLER(sampler_TextureSample);
                float4 _TextureSample_ST;
            CBUFFER_END
            TEXTURE2D(_TextureSample);

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 posWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionHCS = TransformWorldToHClip(ApplyShadowBias(posWS, normalize(mul((float3x3)unity_ObjectToWorld, float3(0,1,0))), _LightDirection));
                float2 uv = TRANSFORM_TEX(input.uv, _TextureSample);
                half4 tex = SAMPLE_TEXTURE2D(_TextureSample, sampler_TextureSample, uv);
                output.alpha = tex.a * input.color.a;
                return output;
            }

            half4 frag(Varyings i) : SV_Target
            {
                clip(i.alpha - 0.01);
                return 0;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor ""
}

/*ASEBEGIN
Version=19801
Node;AmplifyShaderEditor.VertexColorNode;3;-900.6292,-292.9637;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;4;-930.9111,-105.7614;Inherit;False;Property;_ExtraGlow;Extra Glow;1;0;Create;True;0;0;0;False;0;False;0;3;0;15;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-626.4273,-174.2883;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;6;-739.84,71.7299;Inherit;False;Property;_Tint;Tint;0;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,1;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-441.4772,47.30261;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;9;-725.2816,318.2069;Inherit;True;Property;_TextureSample;Texture Sample;2;0;Create;True;0;0;0;False;0;False;-1;None;42db998579832c148abb2fefeaa9946b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-198.9482,49.04736;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-246.6588,288.998;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2.6,23.4;Float;False;True;-1;2;AmplifyShaderEditor.MaterialInspector;0;0;Unlit;Archanor VFX/Retro Arsenal/BasicGlow;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;3;0
WireConnection;7;1;4;0
WireConnection;8;0;7;0
WireConnection;8;1;6;0
WireConnection;10;0;8;0
WireConnection;10;1;9;0
WireConnection;11;0;3;4
WireConnection;11;1;9;4
WireConnection;0;2;10;0
WireConnection;0;9;11;0
ASEEND*/
//CHKSM=362C2C21A46AC9EA4F1D1D50D8645385AF5C9101