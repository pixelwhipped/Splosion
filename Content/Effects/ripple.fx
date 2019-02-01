uniform extern texture ScreenTexture;	

sampler ScreenS = sampler_state
{
	Texture = <ScreenTexture>;	
};

float wave;				// pi/.75 is a good default
float distortion;		// 1 is a good default
float2 centerCoord;		// 0.5,0.5 is the screen center

float4 PixelShaderFunction(float4 position : SV_Position, float4 color : COLOR0, float2 uv: TEXCOORD0) : COLOR
{
	float2 distance = abs(uv - centerCoord);
	float scalar = length(distance);

	// invert the scale so 1 is centerpoint
	scalar = abs(1 - scalar);
		
	// calculate how far to distort for this pixel	
	float sinoffset = sin(wave / scalar);
	sinoffset = clamp(sinoffset, 0, 1);
	
	// calculate which direction to distort
	float sinsign = cos(wave / scalar);	
	
	// reduce the distortion effect
	sinoffset = sinoffset * distortion/32;
	
	// pick a pixel on the screen for this pixel, based on
	// the calculated offset and direction
	return tex2D(ScreenS, uv+(sinoffset*sinsign));	
			

}
technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_4_0_level_9_3 PixelShaderFunction();
    }
}