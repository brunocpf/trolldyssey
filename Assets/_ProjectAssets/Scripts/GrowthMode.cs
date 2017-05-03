using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GrowthMode
{
    Constant,
    Slow,
    Normal,
    Fast
}

static class GrowthModeMethods
{
    public static int GetValueAt(this GrowthMode mode, int x, int c, int maxValue, int max, int cap)
    {
		int value = 0;
        switch (mode)
        {
            case GrowthMode.Constant:
				value = c;
				break;
            case GrowthMode.Slow:
				value = (((int) ((maxValue/max) * 1.5)) * x) + c;
				break;
			case GrowthMode.Normal:
				value = (((int) (maxValue/max)) * x) + c;
				break;
			case GrowthMode.Fast:
				value = (((int) ((maxValue/max) / 1.5)) * x) + c;
				break;
            default:
                value = 0;
				break;
        }
		return Mathf.Clamp(value, 0, cap);
    }
}

