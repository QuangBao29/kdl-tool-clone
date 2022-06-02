//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.8.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/ShaderoOutline"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_BlurHQ_Intensity_1("_BlurHQ_Intensity_1", Range(1, 16)) = 1.001527
_FillColor_Color_1("_FillColor_Color_1", COLOR) = (0,1,0.173913,1)
_OperationBlend_Fade_1("_OperationBlend_Fade_1", Range(0, 1)) = 1
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

// required for UI.Mask
[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _SpriteFade;
float _BlurHQ_Intensity_1;
float4 _FillColor_Color_1;
float _OperationBlend_Fade_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float4 UniColor(float4 txt, float4 color)
{
txt.rgb = lerp(txt.rgb,color.rgb,color.a);
return txt;
}
float4 BlurHQ(float2 uv, sampler2D source, float Intensity)
{
float step1 = 0.00390625f * Intensity * 0.5;
float step2 = step1 * 2;
float4 result = float4 (0, 0, 0, 0);
float2 texCoord = float2(0, 0);
texCoord = uv + float2(-step2, -step2); result += tex2D(source, texCoord);
texCoord = uv + float2(-step1, -step2); result += 4.0 * tex2D(source, texCoord);
texCoord = uv + float2(0, -step2); result += 6.0 * tex2D(source, texCoord);
texCoord = uv + float2(step1, -step2); result += 4.0 * tex2D(source, texCoord);
texCoord = uv + float2(step2, -step2); result += tex2D(source, texCoord);
texCoord = uv + float2(-step2, -step1); result += 4.0 * tex2D(source, texCoord);
texCoord = uv + float2(-step1, -step1); result += 16.0 * tex2D(source, texCoord);
texCoord = uv + float2(0, -step1); result += 24.0 * tex2D(source, texCoord);
texCoord = uv + float2(step1, -step1); result += 16.0 * tex2D(source, texCoord);
texCoord = uv + float2(step2, -step1); result += 4.0 * tex2D(source, texCoord);
texCoord = uv + float2(-step2, 0); result += 6.0 * tex2D(source, texCoord);
texCoord = uv + float2(-step1, 0); result += 24.0 * tex2D(source, texCoord);
texCoord = uv; result += 36.0 * tex2D(source, texCoord);
texCoord = uv + float2(step1, 0); result += 24.0 * tex2D(source, texCoord);
texCoord = uv + float2(step2, 0); result += 6.0 * tex2D(source, texCoord);
texCoord = uv + float2(-step2, step1); result += 4.0 * tex2D(source, texCoord);
texCoord = uv + float2(-step1, step1); result += 16.0 * tex2D(source, texCoord);
texCoord = uv + float2(0, step1); result += 24.0 * tex2D(source, texCoord);
texCoord = uv + float2(step1, step1); result += 16.0 * tex2D(source, texCoord);
texCoord = uv + float2(step2, step1); result += 4.0 * tex2D(source, texCoord);
texCoord = uv + float2(-step2, step2); result += tex2D(source, texCoord);
texCoord = uv + float2(-step1, step2); result += 4.0 * tex2D(source, texCoord);
texCoord = uv + float2(0, step2); result += 6.0 * tex2D(source, texCoord);
texCoord = uv + float2(step1, step2); result += 4.0 * tex2D(source, texCoord);
texCoord = uv + float2(step2, step2); result += tex2D(source, texCoord);
result = result*0.00390625;
return result;
}
float4 OperationBlend(float4 origin, float4 overlay, float blend)
{
float4 o = origin; 
o.a = overlay.a + origin.a * (1 - overlay.a);
o.rgb = (overlay.rgb * overlay.a + origin.rgb * origin.a * (1 - overlay.a)) / (o.a+0.0000001);
o.a = saturate(o.a);
o = lerp(origin, o, blend);
return o;
}
float4 frag (v2f i) : COLOR
{
float4 SourceRGBA_1 = tex2D(_MainTex, i.texcoord);
float4 _BlurHQ_1 = BlurHQ(i.texcoord,_MainTex,_BlurHQ_Intensity_1);
float4 FillColor_1 = UniColor(_BlurHQ_1,_FillColor_Color_1);
float4 OperationBlend_1 = OperationBlend(FillColor_1, SourceRGBA_1, _OperationBlend_Fade_1); 
float4 FinalResult = OperationBlend_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
