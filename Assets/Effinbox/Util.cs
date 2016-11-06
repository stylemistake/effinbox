using UnityEngine;

public static class Util {

    public const float MpsToKph = 3.6f;
    public const float KphToMps = 1 / 3.6f;

    public static float Kph(float value) {
        return value / KphToMps;
    }

    public static float RelativeRangeValue(float value, float start, float end) {
        // Normal range
        if (start < end) {
            if (value < start) {
                return 0;
            }
            if (value > end) {
                return 1;
            }
            return (value - start) / Mathf.Abs(end - start);
        }
        // Inverted range
        if (start > end) {
            if (value > start) {
                return 0;
            }
            if (value < end) {
                return 1;
            }
            return 1 + (end - value) / Mathf.Abs(end - start);
        }
        // Null range
        return 0;
    }

}
