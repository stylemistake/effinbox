using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class AircraftControl: MonoBehaviour {

    public AircraftPhysics aircraft;

	// Use this for initialization
	public void Start() {
        aircraft = GetComponent<AircraftPhysics>();
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
	}

}
