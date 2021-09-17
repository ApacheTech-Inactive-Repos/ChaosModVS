#define PI			3.1415927
#define TAU			6.28218530718
#define HALFPI		1.5707964
#define DEG2RAD		0.017453292
#define RAD2DEG		57.295776

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