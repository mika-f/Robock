struct VS_OUTPUT
{
    float4 Pos: SV_POSITION;
    float2 Tex: TEXCOORD;
};

VS_OUTPUT VS(float4 Pos : POSITION, float2 Tex : TEXCOORD)
{
    VS_OUTPUT vs_output = (VS_OUTPUT)0;
    vs_output.Pos = Pos;
    vs_output.Tex = Tex;

    return vs_output;
}