using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class AircraftPhysics: MonoBehaviour {

    [HideInInspector]
    public Vector3 velocity = Vector3.zero;

    [HideInInspector]
    public Vector3 angularVelocity = Vector3.zero;

    public float mass = 16000;

    public float dragNominal = 90;

    public float speedNominal = 1000;
    public float speedMax = 2000;
    public float speedStall = 500;

    public Knob rollKnob = Knob.CreateBipolar(4.0f);
    public float rollRate = 120;

    public Knob pitchKnob = Knob.CreateBipolar(4.0f);
    public float pitchRate = 60;

    public Knob yawKnob = Knob.CreateBipolar(4.0f);
    public float yawRate = 30;

    public Knob thrustKnob = new Knob(0.6f, 0.25f, 0.2f, 1.0f);
    public float thrustPower = 180;

    public Knob brakesKnob = new Knob(0.0f, 1.5f, 0.0f, 1.0f);
    public float brakesDrag = 0.1f;

    // Use this for initialization
    public void Start() {
        velocity = transform.forward * speedStall * 0.5f;
    }

    // Update is called once per frame
    public void Update() {
        // Read input for the pitch, yaw, roll and throttle of the aeroplane.
        float inputRoll = CrossPlatformInputManager.GetAxis("Horizontal");
        float inputPitch = CrossPlatformInputManager.GetAxis("Vertical");
        bool inputThrottle = CrossPlatformInputManager.GetButton("Fire1");
        bool inputBrakes = CrossPlatformInputManager.GetButton("Fire2");

        // Apply controls
        ApplyHeadingControl(inputPitch, inputRoll, 0.0f);
        ApplySpeedControl(inputThrottle, inputBrakes);

        // Update state
        UpdateVelocity();
        UpdateHeading();

        // Move aircraft
        transform.position += velocity * Time.deltaTime;

        // Show debug info
        ShowDebugInfo();
    }

    public void ApplySpeedControl(bool accel, bool decel) {
        if (accel) {
            thrustKnob.ApplyMax();
            brakesKnob.ApplyMin();
            return;
        }
        if (decel) {
            thrustKnob.ApplyMin();
            brakesKnob.ApplyMax();
            return;
        }
        thrustKnob.ApplyNominal();
        brakesKnob.ApplyNominal();
    }

    public void ApplyHeadingControl(float pitch, float roll, float yaw) {
        pitchKnob.Apply(pitch);
        rollKnob.Apply(roll);
        yawKnob.Apply(yaw);
        var rotationVec = new Vector3(
            pitchKnob.value * pitchRate, 0,
            -rollKnob.value * rollRate);
        var stallQ = Util.RelativeRangeValue(GetSpeed(), 0, speedStall);
        var speedQ = speedNominal / Mathf.Clamp(GetSpeed(), speedStall, speedMax);
        Debug.Log(stallQ * speedQ);
        rotationVec *= Mathf.Clamp(stallQ * speedQ, 0.1f, 1.0f);
        transform.Rotate(rotationVec * Time.deltaTime);
    }

    public void UpdateVelocity() {
        var heading = GetHeading();
        var speed = GetSpeed();
        var drag = GetDrag();
        var stallQ = Util.RelativeRangeValue(speed, 0, speedStall);

        // Adjust velocity based on current heading and speed
        velocity = Vector3.SlerpUnclamped(velocity,
            heading.normalized * velocity.magnitude,
            stallQ * 1.5f * Time.deltaTime);

        // Adjust velocity based on current drag
        velocity -= velocity.normalized * drag * Time.deltaTime;
        velocity += heading * thrustKnob.value * thrustPower * Time.deltaTime;
        velocity -= velocity * brakesKnob.value * brakesDrag * Time.deltaTime;

        // Gravity + Lift
        var gravity = Vector3.down * 98f;
        var lift = transform.up * 98f * speed / speedStall;
        var gravityLift = gravity + lift;
        if (gravityLift.y > 0) {
            gravityLift.y = 0;
        }
        velocity += gravityLift * Time.deltaTime;
    }

    public void UpdateHeading() {
        var heading = GetHeading();
        var speed = GetSpeed();

        // Calculate nose fall
        var fallQ = Util.RelativeRangeValue(speed,
            speedStall * 0.75f, 0.25f);
        heading = Vector3.RotateTowards(heading, Vector3.down,
            fallQ * 1.0f * Time.deltaTime, 0.0f);

        // Calculate damping effect
        heading += velocity.normalized * 1.0f * Time.deltaTime;

        SetHeading(heading);
    }

    public float GetSpeed() {
        return velocity.magnitude;
    }

    public float GetDrag() {
        var speedCoef = GetSpeed() / speedNominal;
        var sideDrag = GetSpeed() - Vector3.Dot(transform.forward, velocity);
        var frontDrag = speedCoef * dragNominal;
        return frontDrag + sideDrag * 1.0f;
    }

    public Vector3 GetHeading() {
        return transform.forward;
    }

    public void SetHeading(Vector3 heading) {
        transform.rotation = Quaternion.LookRotation(heading, transform.up);
    }

    public float GetPitchDegrees() {
        var pitch = transform.rotation.eulerAngles.x;
        return Mathf.PingPong(pitch, 180) * (pitch < 180 ? -1 : 1);
    }

    public void OnGUI() {
        GUI.Label(new Rect(10, 10, 800, 20), "Speed: " + GetSpeed());
        GUI.Label(new Rect(10, 30, 800, 20), "Drag: " + GetDrag());
        GUI.Label(new Rect(10, 50, 800, 20), "Thrust: " + thrustKnob.value);
        GUI.Label(new Rect(10, 70, 800, 20), "Brakes: " + brakesKnob.value);
        GUI.Label(new Rect(10, 90, 800, 20), "Altitude: " + transform.position.y);
        GUI.Label(new Rect(10, 110, 800, 20), "Pitch: " + GetPitchDegrees());
    }

    public void ShowDebugInfo() {
        var cameraTfm = GetComponentInChildren<Camera>().transform;
        Debug.DrawLine(cameraTfm.position + cameraTfm.forward, cameraTfm.position + velocity);
    }

}
