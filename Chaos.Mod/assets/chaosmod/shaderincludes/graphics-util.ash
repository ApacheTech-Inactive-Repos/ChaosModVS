vec4 RGBtoHSL(vec4 rgb) {
	vec4 hsl = vec4(0.0, 0.0, 0.0, rgb.w);
	
	float vMin = min(min(rgb.x, rgb.y), rgb.z);
	float vMax = max(max(rgb.x, rgb.y), rgb.z);
	float vDelta = vMax - vMin;
	
	hsl.z = (vMax + vMin) / 2.0;
	
	if (vDelta == 0.0) {
		hsl.x = hsl.y = 0.0;
	}
	else {
		if (hsl.z < 0.5) hsl.y = vDelta / (vMax + vMin);
		else hsl.y = vDelta / (2.0 - vMax - vMin);
		
		float rDelta = (((vMax - rgb.x) / 6.0) + (vDelta / 2.0)) / vDelta;
		float gDelta = (((vMax - rgb.y) / 6.0) + (vDelta / 2.0)) / vDelta;
		float bDelta = (((vMax - rgb.z) / 6.0) + (vDelta / 2.0)) / vDelta;
		
		if (rgb.x == vMax) hsl.x = bDelta - gDelta;
		else if (rgb.y == vMax) hsl.x = (1.0 / 3.0) + rDelta - bDelta;
		else if (rgb.z == vMax) hsl.x = (2.0 / 3.0) + gDelta - rDelta;
		
		if (hsl.x < 0.0) hsl.x += 1.0;
		if (hsl.x > 1.0) hsl.x -= 1.0;
	}
	
	return hsl;
}

float hueToRGB(float v1, float v2, float vH) {
	if (vH < 0.0) vH+= 1.0;
	if (vH > 1.0) vH -= 1.0;
	if ((6.0 * vH) < 1.0) return (v1 + (v2 - v1) * 6.0 * vH);
	if ((2.0 * vH) < 1.0) return (v2);
	if ((3.0 * vH) < 2.0) return (v1 + (v2 - v1) * ((2.0 / 3.0) - vH) * 6.0);
	return v1;
}

vec4 HSLtoRGB(vec4 hsl) {
	vec4 rgb = vec4(0.0, 0.0, 0.0, hsl.w);
	
	if (hsl.y == 0) {
		rgb.xyz = hsl.zzz;
	}
	else {
		float v1;
		float v2;
		
		if (hsl.z < 0.5) v2 = hsl.z * (1 + hsl.y);
		else v2 = (hsl.z + hsl.y) - (hsl.y * hsl.z);
		
		v1 = 2.0 * hsl.z - v2;
		
		rgb.x = hueToRGB(v1, v2, hsl.x + (1.0 / 3.0));
		rgb.y = hueToRGB(v1, v2, hsl.x);
		rgb.z = hueToRGB(v1, v2, hsl.x - (1.0 / 3.0));
	}
	
	return rgb;
}

vec3 RGBtoHSV(vec3 c)
{
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 RGBtoHSV(float r, float g, float b)
{
    return RGBtoHSV(vec3(r, g, b));
}