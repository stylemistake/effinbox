using UnityEngine;

namespace Effinbox {

  public class RotatableSurface: MonoBehaviour {

    public Vector3 pivotPoint1 = Vector3.zero;
    public Vector3 pivotPoint2 = Vector3.zero;

    [RangeAttribute(0f, 360f)]
    public float lowerLimit = 90f;
    [RangeAttribute(0f, 360f)]
    public float upperLimit = 90f;
    [RangeAttribute(-180f, 180f)]
    public float offsetAngle = 0f;

    public bool invert = false;

    private Vector3 point;
    private Vector3 axis;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public void Start() {
      point = transform.parent.InverseTransformPoint(
        transform.TransformPoint(pivotPoint1));
      axis = transform.parent.InverseTransformDirection(
        transform.TransformPoint(pivotPoint2)
          - transform.TransformPoint(pivotPoint1));
      initialPosition = transform.localPosition;
      initialRotation = transform.localRotation;
    }

    public void Update() {

    }

    public void Rotate(float angle) {
      angle = Mathf.Clamp(angle, -1, 1);
      transform.localPosition = initialPosition;
      transform.localRotation = initialRotation;
      var targetAngle = angle > 0
        ? angle * upperLimit
        : angle * lowerLimit;
      if (invert) {
        targetAngle *= -1;
      }
      transform.RotateAround(
        transform.parent.TransformPoint(point),
        transform.parent.TransformDirection(axis),
        offsetAngle + targetAngle);
    }

  }

}
