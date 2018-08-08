struct VS_OUTPUT
{
    float4 Pos: SV_POSITION;
    float2 Tex: TEXCOORD;
};

VS_OUTPUT VS(float Pos : POSITION, float2 Tex : TEXCOORD)
{
    VS_OUTPUT vertex = (VS_OUTPUT)0;
    vertex.Pos = Pos;
    vertex.Tex = Tex;

    return vertex;
}