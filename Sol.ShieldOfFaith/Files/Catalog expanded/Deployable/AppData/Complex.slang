// This is a minimal example of a custom shader you can import into ShaderGlass
// Full specification and examples can be found at https://github.com/libretro/slang-shaders/blob/master/README.md

#version 450

//////////////////////////////////////////////////////////////////
// you can define parameters of your shader in this section

layout(push_constant) uniform Push
{
    // built-in parameters
    vec4 SourceSize;
    vec4 OriginalSize;
    vec4 OutputSize;
    uint FrameCount;

    // your custom parameters
    float TintRed;
    float TintGreen;
    float TintBlue;
	float TintIntensity;

    float DimIntensity;
    float DimLayer;

    float DesaturateIntensity;
    float DesaturateLayer;

    float LowEndPreservationPeak;
    float LowEndPreservationVolume;
    float PreserveLayer;

} params;

#pragma parameter TintRed "TintRed" 0.0 0.0 1.0 0.005
#pragma parameter TintGreen "TintGreen" 0.0 0.0 1.0 0.005
#pragma parameter TintBlue "TintBlue" 0.0 0.0 1.0 0.005
#pragma parameter TintIntensity "TintIntensity" 0.3 0.0 0.9 0.005

#pragma parameter DimIntensity "DimIntensity" 0.05 0.0 0.99 0.005
#pragma parameter DimLayer "DimLayer" 4.0 0.9 7.1 1.0

#pragma parameter DesaturateIntensity "DesaturateIntensity" 0.0 0.0 1.0 0.005
#pragma parameter DesaturateLayer "DesaturateLayer" 4.0 0.9 7.1 1.0

#pragma parameter LowEndPreservationPeak "LowEndPreservationVolume" 0.0 0.0 1.0 0.005
#pragma parameter LowEndPreservationVolume "LowEndPreservationVolume" 0.0 0.0 1.5 0.002
#pragma parameter PreserveLayer "PreserveLayer" 1.0 0.9 7.1 1.0

layout(std140, set = 0, binding = 0) uniform UBO
{
    mat4 MVP;
} global;

//////////////////////////////////////////////////////////////////
// vertex shader, you usually don't need to modify it

#pragma stage vertex

layout(location = 0) in vec4 Position;
layout(location = 1) in vec2 TexCoord;
layout(location = 0) out vec2 vTexCoord;

void main()
{
   gl_Position = global.MVP * Position;
   vTexCoord = TexCoord;
}

//////////////////////////////////////////////////////////////////
// fragment shader is where you define per-pixel operations

#pragma stage fragment

layout(location = 0) in vec2 vTexCoord;
layout(location = 0) out vec4 FragColor;
layout(set = 0, binding = 2) uniform sampler2D Source;

void main()
{
   // this is the input pixel
   vec3 input_pixel = texture(Source, vTexCoord).rgb;

   bool to_tint = params.TintIntensity > 0;
   bool to_dim = params.DimIntensity > 0;
   bool to_desaturate = params.DesaturateIntensity > 0;
   bool to_preserve = params.LowEndPreservationPeak > 0 && params.LowEndPreservationVolume > 0;

   vec3 preserved = vec3(0.0);
   vec3 im = vec3(0.0);

   for (float layer = 1.001; layer < 7.1; layer += 1.001)
   {
        if (to_preserve && layer > params.PreserveLayer)
        {
            to_preserve = false;
            
            float peak = max(input_pixel.r, max(input_pixel.g, input_pixel.b)) / params.LowEndPreservationPeak;
            float factor = (input_pixel.r + input_pixel.g + input_pixel.b) / params.LowEndPreservationVolume;

            if (peak > factor) factor = peak;

            if (factor > 1) {
                preserved = input_pixel / factor;
                input_pixel -= preserved;
            } else {
                to_tint = false;
                to_dim = false;
                to_desaturate = false;
            }
        }

        int divby = 0;

        if (to_tint && layer > 3.0) {
            to_tint = false;
            im += input_pixel * (1 - params.TintIntensity) +
                vec3(input_pixel.r * params.TintRed, input_pixel.g * params.TintGreen, input_pixel.b * params.TintBlue) * params.TintIntensity;
            divby = 1;
        }

        if (to_dim && layer > params.DimLayer) {
            to_dim = false;
            im += (1 - sqrt(1 - input_pixel)) * params.DimIntensity + input_pixel * (1 - params.DimIntensity);
            divby += 1;
        }

        if (to_desaturate && layer > params.DesaturateLayer)
        {
            to_desaturate = false;
            im += vec3((input_pixel.r + input_pixel.g + input_pixel.b) * params.DesaturateIntensity / 3.0) + 
                input_pixel * (1 - params.DesaturateIntensity);
            divby += 1;
        }

        if (divby > 0)
        {
            input_pixel = im / divby;
            divby = 0;
            im = vec3(0.0);
        }
   }

   FragColor = vec4(input_pixel + preserved, 1.0);
}
