#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
	#define HLSL_4
#endif


//#pragma parameter magnitude "Distortion Magnitude" 0.9 0.0 25.0 0.1
//#pragma parameter always_on "OSD Always On" 0.0 0.0 1.0 1.0

float1 magnitude = 5;


uniform float4x4 modelViewProj;

struct out_vertex
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 texCoord : TEXCOORD0;
};
/*
out_vertex main_vertex(COMPAT_IN_VERTEX)
{
	out_vertex OUT;
#ifdef HLSL_4
	float4 position = VIN.position;
	float2 texCoord = VIN.texCoord;
#else
	OUT.Color = color;
#endif
	OUT.position = mul(modelViewProj, position);
	OUT.texCoord = texCoord;
	
	return OUT;
}*/
/*
float rand(float2 co)
{
     float a = 12.9898;
     float b = 78.233;
     float c = 43758.5453;
     float dt= dot(co.xy ,float2(a,b));
     float sn= fmod(dt,3.14);
    return frac(sin(sn) * c);
}
*/

float rand(float2 co) {
	return(frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453)) * 1;
}
//random hash
float4 hash42(float2 p){
    
	float4 p4 = frac(float4(p.xyxy) * float4(443.8975,397.2973, 491.1871, 470.7827));
    p4 += dot(p4.wzxy, p4+19.19);
    return frac(float4(p4.x * p4.y, p4.x*p4.z, p4.y*p4.w, p4.x*p4.w));
}

float hash( float n ){
    return frac(sin(n)*43758.5453123);
}

// 3d noise function (iq's)
float n( in float3 x ){
    float3 p = floor(x);
    float3 f = frac(x);
    f = f*f*(3.0-2.0*f);
    float n = p.x + p.y*57.0 + 113.0*p.z;
    float res = lerp(lerp(lerp( hash(n+  0.0), hash(n+  1.0),f.x),
                        lerp( hash(n+ 57.0), hash(n+ 58.0),f.x),f.y),
                    lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
                        lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
    return res;
}

//tape noise
float nn(float2 p, float framecount){


    float y = p.y;
    float s = fmod(framecount * 0.15, 4837.0);
    
    float v = (n( float3(y*.01 +s, 			1., 1.0) ) + .0)
          	 *(n( float3(y*.011+1000.0+s, 	1., 1.0) ) + .0) 
          	 *(n( float3(y*.51+421.0+s, 	1., 1.0) ) + .0)   
        ;
   	v*= hash42(   float2(p.x +framecount*0.01, p.y) ).x +.3 ;

    
    v = pow(v+.3, 1.);
	if(v<.99) v = 0.;  //threshold
    return v;
}

float3 distort(sampler2D DecalSampler, float2 uv, float size, float framecount){
	float mag = size * 0.0001;

	float3 offset_x = float3(uv.x,uv.x,uv.x);
	offset_x.x = /*rand(float2(fmod(framecount, 9847.0) * 0.03, uv.y * 0.42)) * 0.001*/  rand(float2(sin(framecount) * 0.001, uv.y)) * mag;
	offset_x.y = /* rand(float2(fmod(framecount, 5583.0) * 0.004, uv.y * 0.002)) * 0.004*/ +rand(float2(cos(-framecount+100) *   0.004, uv.y)) * mag;
	offset_x.z = rand(float2(sin(-framecount+1000) *  0.002, uv.y)) * mag;
	
	
	return float3(tex2D(DecalSampler, float2(uv.x+offset_x.x, uv.y-(offset_x.x)/2)).r,
				tex2D(DecalSampler, float2(uv.x+offset_x.y, uv.y-(offset_x.y)/2)).g,
				tex2D(DecalSampler, float2(uv.x+offset_x.z, uv.y-(offset_x.z)/2)).b);
}

float onOff(float a, float b, float c, float framecount)
{
	return step(c, sin((framecount * 0.001) + a*cos((framecount * 0.001)*b)));
}

float2 jumpy(float2 uv, float framecount)
{
	float2 look = uv;
	float window = 1./(1.+80.*(look.y-fmod(framecount/4.,1.))*(look.y-fmod(framecount/4.,1.)));
	look.x += 0.05 * sin(look.y*10. + framecount)/20.*onOff(4.,4.,.3, framecount)*(0.5+cos(framecount*20.))*window;
	float vShift = 0.4*onOff(2.,3.,.9, framecount)*(sin(framecount)*sin(framecount*20.) + 
										 (0.5 + 0.1*sin(framecount*200.)*cos(framecount)));
	look.y = fmod(look.y - 0.01 * vShift, 1.);
	return look;
}


float2 textureSize;
float2 videoSize;
float1 fps;
float1 staticAlpha = 0.05;

float4 loose_connection(float2 texture_size, float2 video_size, float frame_count, float2 texCoord, sampler2D decal,sampler2D overlay)
{
	float2 LUTeffectiveCoord = float2(frac(texCoord.xy * texture_size.xy / video_size.xy));
	float timer = frame_count;
	float3 res = distort(decal, jumpy(texCoord, timer), magnitude, timer);
	float col = nn(-texCoord * video_size.y * 4.0, timer);
	float3 play = distort(overlay, jumpy(LUTeffectiveCoord, timer), magnitude, timer);
	float overlay_alpha = tex2D(overlay, jumpy(LUTeffectiveCoord, timer)).a;
	float show_overlay = clamp(overlay_alpha, 0.0, 0.8);
	res = lerp(res, play, show_overlay);
 
	float4 final = float4(res + clamp(float3(col, col, col), 0.0,staticAlpha), 1.0);
	return final;
}




Texture2D decal;
sampler2D DecalSampler = sampler_state
{
	Texture = <decal>;
};
Texture2D overlay;
sampler2D overlaySampler = sampler_state
{
	Texture = <overlay>;
};


float4 main_fragment(out_vertex VOUT) : COLOR0
{
	return loose_connection(textureSize,videoSize , fps, VOUT.texCoord, DecalSampler,overlaySampler);
}

technique
{
    pass
	{
	    PixelShader = compile PS_SHADERMODEL main_fragment();
	}
}

