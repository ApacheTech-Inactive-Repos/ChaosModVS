#version 330 core
#extension GL_ARB_explicit_attrib_location : enable
layout(location = 0) in vec3 vertex;

#include maths-util.ash

// Generic vertex shader that translates screen UV co-ordinates to clip space,
// for use with screen overlay post-processing fragment shaders.

//? HACK: Not yet implemented variables, for use when creating distortion/refraction effects within the screen UV.

uniform float dt;           // capi.World.ElapsedInGameMilliseconds.
uniform float rand;         // Pseudo-random number between 0.0f and 1.0f
uniform int distort;        // Boolean value to say whether to add distortion to pixels or not.

out vec2 uv;

void main(void)
{
    // Translate screen UV to clip space.
    gl_Position = vec4(vertex.xy, 0, 1);

    // Centre clip space origin, on screen.
    uv = (vertex.xy + 1.0) / 2.0;
}