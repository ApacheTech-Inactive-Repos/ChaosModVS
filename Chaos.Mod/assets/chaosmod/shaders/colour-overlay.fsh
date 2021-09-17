#version 330 core

// Includes
#include maths-util.ash
#include colour-util.ash

// Constant Definitions
#define NONE		0
#define RED			1
#define GREEN		2
#define BLUE		3
#define GREYSCALE	4
#define SEPIA		5
#define RAINBOW		6

#define TAU 6.28218530718   // TAU = 2 * PI.

// Basic Shader Properties
uniform sampler2D primaryScene;
uniform int filter;
uniform float intensity;
uniform float brightness;
uniform int compress;

// Animated Shader Properites
uniform float dt;
uniform float saturation;
uniform float luminosity;
uniform float spread;
uniform float speed;

uniform float rand;

// Inputs from Vertex Shader
in vec2 uv;

// Output to Screen
out vec4 outColour;

void main()
{
	vec4 fCol = texture(primaryScene, uv);
	vec3 mixColour = fCol.rgb;

	if (filter == RED)
	{
		mixColour = vec3(
			(fCol.r * 0.2126) + (fCol.g * 0.7152) + (fCol.b * 0.0722),
			0.0f,
			0.0f);
	}
	else if (filter == GREEN)
	{
		mixColour = vec3(
			0.0f,
			(fCol.r * 0.2126) + (fCol.g * 0.7152) + (fCol.b * 0.0722),
			0.0f);
	}
	else if (filter == BLUE)
	{
		mixColour = vec3(
			0.0f,
			0.0f,
			(fCol.r * 0.2126) + (fCol.g * 0.7152) + (fCol.b * 0.0722));
	}
	else if (filter == GREYSCALE)
	{
		mixColour = vec3(dot(fCol.rgb, vec3(0.2126, 0.7152, 0.0722)));
	}
	else if (filter == SEPIA)
	{
		mixColour = vec3(
			(fCol.r * 0.393) + (fCol.g * 0.769) + (fCol.b * 0.189),
			(fCol.r * 0.349) + (fCol.g * 0.686) + (fCol.b * 0.168),
			(fCol.r * 0.272) + (fCol.g * 0.534) + (fCol.b * 0.131));
	}
	else if (filter == RAINBOW)
	{
		vec2 lPos = uv / spread;
		float time = dt * speed / spread;
		float hue = (-lPos.y) / 2.0;
		hue += cos(lPos.y - time * TAU) * 0.5 + 0.5;
		hue = softClamp(hue, 0.0, 1.0, 1.0);
		vec4 hsl = vec4(hue, saturation, luminosity, 1.0);
		mixColour = HSLtoRGB(hsl).rbg;
	}
	else
	{
		mixColour = fCol.rgb;
	}

	float inten = intensity;
	if (compress != 0)
	{
		float bright = ((fCol.r * 0.2126) + (fCol.g * 0.7152) + (fCol.b * 0.0722));
		inten = inten * (1.0 - min(sqrt(bright), 1.0));
	}	
                   
	float scale = 1.0 + brightness * inten;
	outColour.r = min((fCol.r * (1.0 - inten) + mixColour.r * inten) * scale, 1.0);
	outColour.g = min((fCol.g * (1.0 - inten) + mixColour.g * inten) * scale, 1.0);
	outColour.b = min((fCol.b * (1.0 - inten) + mixColour.b * inten) * scale, 1.0);
	outColour.a = fCol.a;
}