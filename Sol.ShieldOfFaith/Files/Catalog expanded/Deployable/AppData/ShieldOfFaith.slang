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
	float Intensity;
    float RedFactor;
	float GreenFactor;
	float BlueFactor;
} params;

#pragma parameter Intensity "Intensity" 0.3 0.0 0.9 0.005
#pragma parameter RedFactor "RedFactor" 0.0 0.0 1.0 0.005
#pragma parameter GreenFactor "GreenFactor" 0.0 0.0 1.0 0.005
#pragma parameter BlueFactor "BlueFactor" 0.0 0.0 1.0 0.005

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
   vec3 blend = vec3(params.RedFactor, params.GreenFactor, params.BlueFactor) * input_pixel;
   FragColor = vec4(input_pixel * (1 - params.Intensity) + blend * params.Intensity, 1.0);
}
