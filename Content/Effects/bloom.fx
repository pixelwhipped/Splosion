float BaseIntensity;
float BaseSaturation;
float BloomIntensity;
float BloomSaturation;

texture SourceTexture;
sampler SourceTextureSampler = sampler_state
{
    Texture = <SourceTexture>;
};

float4 AdjustSaturation(float4 color, float saturation)
{
    float grey = dot(color, float3(0.3f, 0.59f, 0.11f));
    return lerp(grey, color, saturation);
}

float4 PixelShaderFunction(float4 position : SV_Position, float4 color : COLOR0, float2 uv: TEXCOORD0) : COLOR
{

    float BloomThreshold = 0.25f;

    float4 base = tex2D(SourceTextureSampler, uv);
    float4 bloom = saturate((base - BloomThreshold) / (1 - BloomThreshold));
    
    // Adjust color saturation and intensity.
    bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
    base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
    
    // Darken down the base image in areas where there is a lot of bloom,
    // to prevent things looking excessively burned-out.
    base *= (1.0f - saturate(bloom));
    
    // Combine the two images.
    return base + bloom;
}
technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_4_0_level_9_3 PixelShaderFunction();
    }
}