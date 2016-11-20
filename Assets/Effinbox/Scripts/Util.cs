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

    public static float Diff(float value, float diff) {
        if (diff < 0 && value > 0) {
            return value * (1 + diff);
        }
        if (diff > 0 && value < 0) {
            return value * (1 - diff);
        }
        return value;
    }

    public static Vector3 AxisAngles(Transform transform, Vector3 target) {
        return AxisAngles(transform.forward, transform.up, target);
    }

    public static Vector3 AxisAngles(Transform transform, Vector3 target, Vector3 targetUp) {
        return AxisAngles(transform.forward, transform.up, target, targetUp);
    }

    public static Vector3 AxisAngles(Vector3 forward, Vector3 up, Vector3 target) {
        return AxisAngles(forward, up, target, up);
    }

    public static Vector3 AxisAngles(Vector3 forward, Vector3 up, Vector3 target, Vector3 targetUp) {
        var sourceRot = Quaternion.LookRotation(forward, up);
        var targetRot = Quaternion.LookRotation(target.normalized, targetUp);
        var q = Quaternion.Inverse(sourceRot) * targetRot;
        if (q.w > 0.999999) {
            return Vector3.zero;
        }
        var size = Mathf.Sqrt(1 - q.w * q.w);
        var result = new Vector3(q.x, q.y, q.z);
        if (size > 0.01) {
            result /= size;
        }
        result *= q.w >= 0
            ?  Mathf.Sin(Mathf.Acos(q.w))
            : -Mathf.Sin(Mathf.Acos(q.w));
        return -result;
    }

}
