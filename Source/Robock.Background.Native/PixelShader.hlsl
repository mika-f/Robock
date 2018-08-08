Texture2D tex2D : register (t0);
SamplerState sampleLinear : register(s0);

struct VS_INPUT
{
    float4 Pos: SV_POSITION;
    float2 Tex: TEXCOORD;
};

float4 PS(VS_INPUT input) : SV_TARGET
{
    return tex2D.Sample(sampleLinear, input.Tex);
}