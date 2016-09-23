using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class AircraftPhysics: MonoBehaviour {

    public Vector3 velocity = Vector3.zero;
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

    }

    // Update is called once per frame
    public void Update() {
        // Read input for the pitch, yaw, roll and throttle of the aeroplane.
        float inputRoll = CrossPlatformInputManager.GetAxis("Horizontal");
        float inputPitch = CrossPlatformInputManager.GetAxis("Vertical");
        bool inputThrottle = CrossPlatformInputManager.GetButton("Fire1");
        bool inputBrakes = CrossPlatformInputManager.GetButton("Fire2");

        // Apply controls
        this.ApplyHeadingControl(inputPitch, inputRoll, 0.0f);
        this.ApplySpeedControl(inputThrottle, inputBrakes);

        // Update state
        this.UpdateVelocity();
        this.UpdateHeading();

        // Move aircraft
        this.transform.position += this.velocity * Time.deltaTime;

        // Show debug info
        this.ShowDebugInfo();
    }

    public void ApplySpeedControl(bool accel, bool decel) {
        if (accel) {
            this.thrustKnob.ApplyMax();
            this.brakesKnob.ApplyMin();
            return;
        }
        if (decel) {
            this.thrustKnob.ApplyMin();
            this.brakesKnob.ApplyMax();
            return;
        }
        this.thrustKnob.ApplyNominal();
        this.brakesKnob.ApplyNominal();
    }

    public void ApplyHeadingControl(float pitch, float roll, float yaw) {
        this.pitchKnob.Apply(pitch);
        this.rollKnob.Apply(roll);
        this.yawKnob.Apply(yaw);
        var rotationVec = new Vector3(
            this.pitchKnob.value * this.pitchRate, 0,
            -this.rollKnob.value * this.rollRate);
        var stallQ = Util.RelativeRangeValue(this.GetSpeed(),
            this.speedStall * 0.25f, this.speedStall);
        rotationVec *= Mathf.Clamp(stallQ, 0.25f, 1.0f);
        this.transform.Rotate(rotationVec * Time.deltaTime);
    }

    public void UpdateVelocity() {
        var heading = this.GetHeading();
        var speed = this.GetSpeed();
        var drag = this.GetDrag();
        var stallQ = Util.RelativeRangeValue(speed, 0, this.speedStall);

        // Adjust velocity based on current heading and speed
        this.velocity = Vector3.SlerpUnclamped(this.velocity,
            heading.normalized * this.velocity.magnitude,
            stallQ * 1.5f * Time.deltaTime);

        // Adjust velocity based on current drag
        this.velocity -= this.velocity.normalized * drag * Time.deltaTime;
        this.velocity += heading * this.thrustKnob.value * this.thrustPower * Time.deltaTime;
        this.velocity -= this.velocity * this.brakesKnob.value * this.brakesDrag * Time.deltaTime;

        // Gravity + Lift
        var gravity = Vector3.down * 98f;
        var lift = this.transform.up * 98f * speed / this.speedStall;
        var gravityLift = gravity + lift;
        if (gravityLift.y > 0) {
            gravityLift.y = 0;
        }
        this.velocity += gravityLift * Time.deltaTime;
    }

    public void UpdateHeading() {
        var heading = this.GetHeading();
        var speed = this.GetSpeed();

        // Calculate nose fall
        var fallQ = Util.RelativeRangeValue(speed,
            this.speedStall * 0.75f, 0.25f);
        heading = Vector3.RotateTowards(heading, Vector3.down,
            fallQ * 1.0f * Time.deltaTime, 0.0f);

        // Calculate damping effect
        heading += this.velocity.normalized * 1.0f * Time.deltaTime;

        this.SetHeading(heading);
    }

    public float GetSpeed() {
        return this.velocity.magnitude;
    }

    public float GetDrag() {
        var speedCoef = this.GetSpeed() / speedNominal;
        var sideDrag = this.GetSpeed() - Vector3.Dot(this.transform.forward, this.velocity);
        var frontDrag = speedCoef * this.dragNominal;
        return frontDrag + sideDrag * 0.5f;
    }

    public Vector3 GetHeading() {
        return this.transform.forward;
    }

    public void SetHeading(Vector3 heading) {
        this.transform.rotation = Quaternion.LookRotation(heading, this.transform.up);
    }

    public float GetPitchDegrees() {
        var pitch = this.transform.rotation.eulerAngles.x;
        return Mathf.PingPong(pitch, 180) * (pitch < 180 ? -1 : 1);
    }

    public void OnGUI() {
        GUI.Label(new Rect(10, 10, 800, 20), "Speed: " + this.GetSpeed());
        GUI.Label(new Rect(10, 30, 800, 20), "Drag: " + this.GetDrag());
        GUI.Label(new Rect(10, 50, 800, 20), "Thrust: " + this.thrustKnob.value);
        GUI.Label(new Rect(10, 70, 800, 20), "Brakes: " + this.brakesKnob.value);
        GUI.Label(new Rect(10, 90, 800, 20), "Altitude: " + this.transform.position.y);
        GUI.Label(new Rect(10, 110, 800, 20), "Pitch: " + this.GetPitchDegrees());
    }

    public void ShowDebugInfo() {
        var cameraTfm = this.GetComponentInChildren<Camera>().transform;
        Debug.DrawLine(cameraTfm.position + cameraTfm.forward, cameraTfm.position + this.velocity);
    }

}
