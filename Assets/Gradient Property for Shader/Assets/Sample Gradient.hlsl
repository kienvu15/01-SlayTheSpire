#ifndef GRADIENTS_INCLUDED
#define GRADIENTS_INCLUDED

// Samples a vertical gradient texture using a scalar t
float4 SampleGradient(sampler2D gradient, float t)
{
    float2 uv = float2(t, 0.5);
    return tex2D(gradient, uv);
}

#endif