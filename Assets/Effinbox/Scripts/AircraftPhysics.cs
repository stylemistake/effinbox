using UnityEngine;
using System.Collections.Generic;

namespace Effinbox {

  [RequireComponent (typeof (Rigidbody))]
  public class AircraftPhysics: MonoBehaviour {

    // public Vector3 velocity {
    //     get {
    //         return rigidbody == null
    //             ? Vector3.zero
    //             : rigidbody.velocity;
    //     }
    //     set {
    //         rigidbody.velocity = value;
    //     }
    // }

    public float nominalDrag = 0.015f;

    public float speedStall = 138;
    public float speedNominal = 263;
    public float speedMax = 720;

    public Knob rollKnob = Knob.CreateBipolar(4.0f);
    public float rollRate = 1.0f;

    public Knob pitchKnob = Knob.CreateBipolar(4.0f);
    public float pitchRate = 1.2f;

    public Knob yawKnob = Knob.CreateBipolar(4.0f);
    public float yawRate = 0.15f;

    public Knob thrustKnob = new Knob(0.25f, 0.0f, 1.0f, 0.5f);
    public float thrustPower = 300f;

    public Knob brakesKnob = new Knob(0.0f, 0.0f, 1.0f, 1.5f);
    public float brakesDrag = 0.1f;

    private new Rigidbody rigidbody;

    // Use this for initialization
    public void Start() {
      rigidbody = GetComponent<Rigidbody>();
      rigidbody.velocity = transform.forward * speedStall;
    }

    // Update is called once per frame
    public void FixedUpdate() {
      // Update state
      UpdateMovement();
      // UpdateVelocity();
      // UpdateHeading();

      var engineColor = new Color(.23f, .14f, 0f, thrustKnob.value * 1.0f);
      var engineLeft = transform.Find("Body/EngineLeft").GetComponent<Renderer>();
      var engineRight = transform.Find("Body/EngineRight").GetComponent<Renderer>();
      engineLeft.material.SetColor("_TintColor", engineColor);
      engineRight.material.SetColor("_TintColor", engineColor);

      var pitchVal = pitchKnob.value * 25;
      var rollVal = rollKnob.value * 20;
      var yawVal = yawKnob.value * 25;

      transform.Find("Body/ControlSurfaces/Brakes")
        .localEulerAngles = new Vector3(-90 + brakesKnob.value * 80, 0, 180);
      transform.Find("Body/ControlSurfaces/AileronLeft")
        .localEulerAngles = new Vector3(-90 - rollVal - pitchVal, 0, 176);
      transform.Find("Body/ControlSurfaces/AileronRight")
        .localEulerAngles = new Vector3(-90 + rollVal - pitchVal, 0, 184);
      transform.Find("Body/ControlSurfaces/ElevatorLeft")
        .localEulerAngles = new Vector3(-90 - rollVal - pitchVal, 0, 176);
      transform.Find("Body/ControlSurfaces/ElevatorRight")
        .localEulerAngles = new Vector3(-90 + rollVal - pitchVal, 0, 184);
      transform.Find("Body/ControlSurfaces/CanardLeft")
        .localEulerAngles = new Vector3(-90 + pitchVal, 0, 180);
      transform.Find("Body/ControlSurfaces/CanardRight")
        .localEulerAngles = new Vector3(-90 + pitchVal, 0, 180);
      transform.Find("Body/ControlSurfaces/Rudder/RudderMesh")
        .localEulerAngles = new Vector3(-60, - yawVal, -180);

      transform.Find("Body").localEulerAngles = new Vector3(
          pitchKnob.value * 4, yawKnob.value * 2, -rollKnob.value * 8);
    }

    public void ApplySpeedControl(float accel, float decel) {
      if (accel > 0.1) {
        thrustKnob.ApplyMax();
        brakesKnob.ApplyMin();
        return;
      }
      if (decel > 0.1) {
        thrustKnob.ApplyMin();
        brakesKnob.ApplyMax();
        return;
      }
      thrustKnob.ApplyNominal();
      brakesKnob.ApplyNominal();
    }

    public void ApplyHeadingControl(Vector3 axis) {
      ApplyHeadingControl(axis.x, axis.z, axis.y);
    }

    public void ApplyHeadingControl(float pitch, float roll, float yaw) {
      pitchKnob.Apply(pitch);
      rollKnob.Apply(roll);
      yawKnob.Apply(yaw);
    }

    public void ApplyRoll(float value) {
      rollKnob.Apply(value);
    }

    public void ApplyPitch(float value) {
      pitchKnob.Apply(value);
    }

    public void ApplyYaw(float value) {
      yawKnob.Apply(value);
    }

    public void UpdateMovement() {
      // Common vars
      var velocity = rigidbody.velocity;
      var speed = velocity.magnitude;
      var speedQ = speed / speedNominal;
      var speedStallQ = Util.RelativeRangeValue(speed, speedStall * 0.5f, speedStall);

      //  Thrust
      // ----------------------------------------
      var thrust = transform.forward * thrustKnob.value
        * thrustPower * 1000;
      rigidbody.AddForce(thrust);

      //  Handling
      // ----------------------------------------
      float pitchDot = 0;
      float yawDot = 0;
      if (speed > 1) {
        var localHeadingAngles = Util.AxisAngles(transform, velocity.normalized);
        pitchDot = localHeadingAngles.x;
        yawDot = localHeadingAngles.y;
        var rotationVec = new Vector3(
          Util.Diff(pitchKnob.value, -0.5f),
          yawKnob.value,
          -rollKnob.value);
        // Global modifiers
        rotationVec *= speedStallQ;
        if (speedQ > 1) {
          rotationVec.x /= speedQ;
          rotationVec.y /= speedQ;
        }
        // Pitch modifiers
        rotationVec.x *= pitchRate * 4f;
        rotationVec.x *= 1 - Mathf.Abs(pitchDot);
        rotationVec.x -= pitchDot * 5f;
        // Yaw modifiers
        rotationVec.y *= yawRate * 4f;
        rotationVec.y *= 1 - Mathf.Abs(yawDot);
        rotationVec.y -= yawDot * 5f;
        // Roll modifiers
        rotationVec.z *= rollRate * 10f;
        // Apply torque
        rotationVec *= Time.deltaTime;
        rigidbody.AddRelativeTorque(rotationVec, ForceMode.VelocityChange);
      }

      //  Turning effect
      // ----------------------------------------
      rigidbody.velocity = Vector3.SlerpUnclamped(
        rigidbody.velocity,
        transform.forward * rigidbody.velocity.magnitude,
        speedStallQ * 1.5f * Time.deltaTime);

      //  Lift
      // ----------------------------------------
      var gravity = Physics.gravity;
      var lift = transform.up * gravity.magnitude * speedStallQ;
      lift += gravity;
      if (lift.y > 0) {
        lift.y = 0;
      }
      lift -= gravity;
      rigidbody.AddForce(lift, ForceMode.Acceleration);

      //  Drag
      // ----------------------------------------
      // Parasytic drag
      rigidbody.drag = nominalDrag;
      // Wing surface drag
      if (speed > 1) {
        rigidbody.drag += Mathf.Abs(pitchDot) * 0.2f;
        rigidbody.drag += Mathf.Abs(yawDot) * 0.02f;
      }
      // Control surface drag
      rigidbody.drag += brakesKnob.value * brakesDrag;
      rigidbody.drag += rollKnob.magnitude * 0.010f;
      rigidbody.drag += pitchKnob.magnitude * 0.025f;
      rigidbody.drag += yawKnob.magnitude * 0.002f;
      // Static angular drag
      rigidbody.angularDrag = 2f;
      // Speed induced angular drag
      rigidbody.angularDrag += 1f * speedQ;
      // Control surface angular drag
      rigidbody.angularDrag += rollKnob.magnitude * 0.50f;
      rigidbody.angularDrag += pitchKnob.magnitude * 0.75f;
      rigidbody.angularDrag += yawKnob.magnitude * 0.1f;
    }

  }

}
