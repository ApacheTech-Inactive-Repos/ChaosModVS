#version 330 core

/***
 * Thank you to Xandu, for providing a working sample of how
 * to implement shaders into Vintage Story mods. Full credit
 * goes to them, and their XSkills mod, for the single colour
 * overlay filters included within this fragment shader.
 * 
 * XSkills Mod, by Xandu:
 * https://mods.vintagestory.at/show/mod/247
 *
 * Initial animated Rainbow shader by CryCry, for VCRMods:
 * https://vrcmods.com/item/431-Script---Animated-Rainbow-Shader
 **/

// Overly Type Enum Definitions
#define FILTER_NONE			0
#define FILTER_RED			1
#define FILTER_GREEN		2
#define FILTER_BLUE			3
#define FILTER_GREYSCALE	4
#define FILTER_SEPIA		5
#define FILTER_RAINBOW		6

// Inputs from ShaderProgram.
uniform sampler2D	iChannel0;				// Primary Scene Texture
uniform sampler2D	iChannel1;				// Custom 2D Texture
uniform sampler2D	iChannel2;				// Custom 2D Texture
uniform sampler2D	iChannel3;				// Custom 2D Texture
uniform vec2		iChannelResolution0;	// Resolution of iChannel0, in pixels.
uniform vec2		iChannelResolution1;	// Resolution of iChannel1, in pixels.
uniform vec2		iChannelResolution2;	// Resolution of iChannel2, in pixels.
uniform vec2		iChannelResolution3;	// Resolution of iChannel3, in pixels.
uniform float		iChannelTime0;			// Playback Time of iChannel0, in seconds.
uniform float		iChannelTime1;			// Playback Time of iChannel1, in seconds.
uniform float		iChannelTime2;			// Playback Time of iChannel2, in seconds.
uniform float		iChannelTime3;			// Playback Time of iChannel3, in seconds.

uniform vec2		iResolution;			// Resolution of the Game Window, in pixels.
uniform float		iTime;					// Time Ticker, in seconds.
uniform vec4		iMouse;					// Mouse pixel coords. xy: Current (if MLB down), zw: Click
uniform float		iSampleRate;            // Sound sample rate (i.e., 44100)
uniform vec4		iDate;					// Current Date and Time: (x:Year, y:Month, z:Day, w:TimeInSeconds)

uniform float		iBrightness;			// The brightness of the output colour.
uniform int			iCompress;				// Boolean: Whether or not to compress the output colour values.
uniform int			iFilter;				// Enum: The type of filter to use.
uniform float		iIntensity;				// The intensity of the output colour.

uniform float		iLuminosity;			// HSL: The luminosity of the output colour.
uniform float		iSaturation;			// HSL: The saturation of the output colour.
uniform float		iSpeed;					// The speed of the animated texture
uniform float		iSpread;				// The aplitude of the animated texture.

// Inputs from Vertex Shader.
in vec2				fragCoord;				// The centred, clip-space uv coords of the fragment.

// Outputs to GPU.
out vec4			fragColor;				// The final outputted colour of the fragment. Typo kept for compatibility sake.

// Includes
#include maths-util.ash
#include graphics-util.ash
#include effects-util.ash

void main()
{
	vec4 fCol = texture(iChannel0, fragCoord);
	vec3 mixColour = fCol.rgb;

	// ==========================================================================
	//  Determine Overlay Colour
	// ==========================================================================

	if (iFilter == FILTER_RED)
	{
		mixColour = vec3(
			(fCol.r * 0.2126) + (fCol.g * 0.7152) + (fCol.b * 0.0722),
			0.0f,
			0.0f);
	}

	else if (iFilter == FILTER_GREEN)
	{
		mixColour = vec3(
			0.0f,
			(fCol.r * 0.2126) + (fCol.g * 0.7152) + (fCol.b * 0.0722),
			0.0f);
	}

	else if (iFilter == FILTER_BLUE)
	{
		mixColour = vec3(
			0.0f,
			0.0f,
			(fCol.r * 0.2126) + (fCol.g * 0.7152) + (fCol.b * 0.0722));
	}

	else if (iFilter == FILTER_GREYSCALE)
	{
		mixColour = vec3(dot(fCol.rgb, vec3(0.2126, 0.7152, 0.0722)));
	}

	else if (iFilter == FILTER_SEPIA)
	{
		mixColour = vec3(
			(fCol.r * 0.393) + (fCol.g * 0.769) + (fCol.b * 0.189),
			(fCol.r * 0.349) + (fCol.g * 0.686) + (fCol.b * 0.168),
			(fCol.r * 0.272) + (fCol.g * 0.534) + (fCol.b * 0.131));
	}

	else if (iFilter == FILTER_RAINBOW)
	{
		vec2 lPos = fragCoord / iSpread;
		float time = iTime * iSpeed / iSpread;
		float hue = (-lPos.y) / 2.0;
		hue += cos(lPos.y - time * CONST_TAU) * 0.5 + 0.5;
		hue = wrap(hue, 0.0, 1.0);
		vec4 hsl = vec4(hue, iSaturation, iLuminosity, 1.0);
		mixColour = HSLtoRGB(hsl).rbg;
	}
	
	// ==========================================================================
	//  Finalise Output Colour
	// ==========================================================================

	float inten = iIntensity;
	if (iCompress != 0)
	{
		float bright = ((fCol.r * 0.2126) + (fCol.g * 0.7152) + (fCol.b * 0.0722));
		inten = inten * (1.0 - min(sqrt(bright), 1.0));
	}

	float scale = 1.0 + iBrightness * inten;
	fragColor.r = min((fCol.r * (1.0 - inten) + mixColour.r * inten) * scale, 1.0);
	fragColor.g = min((fCol.g * (1.0 - inten) + mixColour.g * inten) * scale, 1.0);
	fragColor.b = min((fCol.b * (1.0 - inten) + mixColour.b * inten) * scale, 1.0);
	fragColor.a = fCol.a;
}