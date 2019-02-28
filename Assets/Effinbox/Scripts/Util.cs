using UnityEngine;

namespace Effinbox {

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

    public static Vector3 AxisAnglesAlt(Transform transform, Vector3 target) {
      return AxisAnglesAlt(transform.forward, transform.up, target);
    }

    public static Vector3 AxisAnglesAlt(Transform transform, Vector3 target, Vector3 targetUp) {
      return AxisAnglesAlt(transform.forward, transform.up, target, targetUp);
    }

    public static Vector3 AxisAnglesAlt(Vector3 forward, Vector3 up, Vector3 target) {
      return AxisAnglesAlt(forward, up, target, up);
    }

    public static Vector3 AxisAnglesAlt(Vector3 forward, Vector3 up,
        Vector3 target, Vector3 targetUp) {
      var sourceRot = Quaternion.LookRotation(forward, up);
      var targetRot = Quaternion.LookRotation(target.normalized, targetUp);
      var rotation = Quaternion.Inverse(sourceRot) * targetRot;
      var angles = rotation.eulerAngles;
      return new Vector3(
        Mathf.PingPong(angles.x, 180) * (angles.x < 180 ? 1 : -1),
        Mathf.PingPong(angles.y, 180) * (angles.y < 180 ? 1 : -1),
        Mathf.PingPong(angles.z, 180) * (angles.z < 180 ? 1 : -1));
    }

    /**
     * Checks whether target position is inside the cone of deadzone,
     * that (very) approximately resembles a cardioid pattern.
     *
     * Cone angle is delta degrees from the right/left of the object, relative
     * to the "forward" vector.
     *
     *   _    ^    _
     *  / \_  |  _/ \
     * |    \ | /    | cone is going sideways
     * |     SRC-----|---->
     * |   _______   |
     *  \_/       \_/
     */
    public static bool InDeadzoneCone(
        Transform source, Vector3 targetPos,
        float coneAngle, float coneSize) {
      return InDeadzoneCone(source.forward, source.position,
        targetPos, coneAngle, coneSize);
    }

    public static bool InDeadzoneCone(
        Vector3 forward, Vector3 sourcePos, Vector3 targetPos,
        float coneAngle, float coneSize) {
      var localPos = targetPos - sourcePos;
      var distance = localPos.magnitude;
      var angle = Vector3.Angle(forward, localPos);
      // Outside of radius
      if (distance > coneSize) {
        return false;
      }
      // Inside the deadzone behind the source
      if (angle > 90 && distance < coneSize * 0.1f) {
        return true;
      }
      // Inside the cone
      return Mathf.Abs(90 - angle) < coneAngle;
    }

    public static bool InRange(float input, float min, float max) {
      return input >= min && input <= max;
    }

    public static bool InRangeAbs(float input, float min, float max) {
      return InRange(Mathf.Abs(input), min, max);
    }

  }

}
