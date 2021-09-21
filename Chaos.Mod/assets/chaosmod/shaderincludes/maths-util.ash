#define CONST_PI			3.1415927
#define CONST_TAU			6.28218530718
#define CONST_HALFPI		1.5707964

#define CONST_DEG2RAD		0.017453292
#define CONST_RAD2DEG		57.295776

float clamp(float val, float min, float max)
{
	if (val < min)
	{
		return min;
	}
	if (val <= max)
	{
		return val;
	}
	return max;
}

float softClamp(float val, float min, float max, float step)
{
	while (val < min) val += step;
	while (val > max) val -= step;
	return val;
}

float n(float g) { return g + 0.5; }

float wrap(float val, float min, float max)
{
    val = val - round((val - min) / (max - min)) * (max - min);
    if (val < 0)
        val = val + max - min;
    return val;
}