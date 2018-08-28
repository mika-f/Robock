Texture2D tex2D : register (t0);
SamplerState sampleLinear : register(s0);

cbuffer ConstantBuffer : register(b0)
{
    float Top;
    float Left;
    float Width;
    float Height;
};

struct VS_INPUT
{
    float4 Pos: SV_POSITION;
    float2 Tex: TEXCOORD;
};

float4 PS(VS_INPUT input) : SV_Target
{
    return tex2D.Sample(sampleLinear, input.Tex * float2(Width, Height) + float2(Left, Top));
}