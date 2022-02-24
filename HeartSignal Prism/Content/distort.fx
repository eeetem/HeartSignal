#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
	#define HLSL_4
#endif



struct out_vertex
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 texCoord : TEXCOORD0;
};



float fps;

sampler2D Sampler = sampler_state{
    AddressU = Wrap;
    AddressV = Wrap;
};


float4 main_fragment(out_vertex VOUT) : COLOR0
{

    
    float x = VOUT.texCoord.x;
	float amplitude = 1.;
    float frequency = 1.;
    float y = sin(x * frequency);
    float t = 0.01*(-fps/50*130.0);
    y += sin(x*frequency*2.1 + t)*4.5;
    y += sin(x*frequency*1.72 + t*1.121)*4.0;
    y += sin(x*frequency*2.221 + t*0.437)*5.0;
    y += sin(x*frequency*3.1122+ t*4.269)*2.5;
    y *= amplitude*0.06;
    
    float4 final = tex2D(Sampler,VOUT.texCoord+ float2(0,y));

	return final;
}

technique
{
    pass P0
	{
	    PixelShader = compile PS_SHADERMODEL main_fragment();
	}
}

