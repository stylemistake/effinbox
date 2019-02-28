using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Effinbox {

  [RequireComponent (typeof (AircraftPhysics))]
  [RequireComponent (typeof (Rigidbody))]
  public class AircraftControl: MonoBehaviour {

    private AircraftPhysics aircraft;
    private Radar radar;
    private new Rigidbody rigidbody;

    // Use this for initialization
    public void Start() {
      aircraft = GetComponent<AircraftPhysics>();
      radar = GetComponent<Radar>();
      rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void FixedUpdate() {
      // Read input for the pitch, yaw, roll and throttle of the aeroplane.
      var inputRoll = CrossPlatformInputManager.GetAxis("Roll");
      var inputPitch = CrossPlatformInputManager.GetAxis("Pitch");
      var inputYaw =  CrossPlatformInputManager.GetAxis("Yaw");
      var inputThrottle = CrossPlatformInputManager.GetAxis("Throttle");
      var inputBrakes = CrossPlatformInputManager.GetAxis("Brakes");

      // Apply controls
      aircraft.ApplyHeadingControl(inputPitch, inputRoll, inputYaw);
      aircraft.ApplySpeedControl(inputThrottle, inputBrakes);

      // Cycle targets
      if (radar) {
        var inputSwitchTarget = CrossPlatformInputManager.GetButtonDown("SwitchTarget");
        if (inputSwitchTarget) {
          radar.CycleTargets();
        }
      }
    }

    public void OnGUI() {
      var i = 0;
      PrintValue(i++, "P. Drag", rigidbody.drag);
      PrintValue(i++, "A. Drag", rigidbody.angularDrag);
      PrintValue(i++, "Thrust", aircraft.thrustKnob.value);
      PrintValue(i++, "Brakes", aircraft.brakesKnob.value);
      PrintValue(i++, "Altitude", transform.position.y);
    }

    public static void PrintValue(int row, string name, string value) {
      GUI.Label(new Rect(10, 10 + row * 18, 800, 20), name + ": " + value);
    }

    public static void PrintValue(int row, string name, float value) {
      GUI.Label(new Rect(10, 10 + row * 18, 800, 20), name + ": " + value);
    }

  }

}
