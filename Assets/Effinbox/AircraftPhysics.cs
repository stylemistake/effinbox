using UnityEngine;

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

    public float dragNominal = 90;

    public float speedNominal = 1000;
    public float speedMax = 2000;
    public float speedStall = 500;

    private Knob rollKnob = Knob.CreateBipolar(2.0f);
    public float rollRate = 120;

    private Knob pitchKnob = Knob.CreateBipolar(2.0f);
    public float pitchRate = 60;

    private Knob yawKnob = Knob.CreateBipolar(2.0f);
    public float yawRate = 30;

    private Knob thrustKnob = new Knob(0.4f, 0.0f, 1.0f, 0.25f);
    public float thrustPower = 180;

    private Knob brakesKnob = new Knob(0.0f, 0.0f, 1.0f, 1.5f);
    public float brakesDrag = 0.1f;

    private new Rigidbody rigidbody;

    // Use this for initialization
    public void Start() {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = transform.forward * speedStall;
    }

    public void LateUpdate() {
        // Show debug info
        ShowDebugInfo();
    }

    // Update is called once per frame
    public void FixedUpdate() {
        // Update state
        UpdateMovement();
        // UpdateVelocity();
        // UpdateHeading();
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

    public void ApplyHeadingControl(float pitch, float roll, float yaw) {
        pitchKnob.Apply(pitch);
        rollKnob.Apply(roll);
        yawKnob.Apply(yaw);
    }

    public void UpdateMovement() {
        // Common vars
        var velocity = rigidbody.velocity;
        var speed = velocity.magnitude;
        var speedQ = speed / speedNominal;
        var speedStallQ = Util.RelativeRangeValue(speed, speedStall * 0.25f, speedStall);
        var heading = velocity.normalized;

        //  Thrust
        // ----------------------------------------
        var turbineEfficiencyQ = 1f + speedQ / 4;
        var thrust = transform.forward * thrustKnob.value
            * thrustPower * 1000;
        rigidbody.AddForce(thrust);

        //  Handling
        // ----------------------------------------
        var rotationVec = new Vector3(
            pitchKnob.value * pitchRate,
            yawKnob.value * yawRate,
            -rollKnob.value * rollRate);
        // Calculate drag caused by control surfaces
        var turnDrag = Mathf.Pow(rotationVec.magnitude / 16, 1.2f);
        rotationVec *= speedStallQ * 0.2f;
        // rotationVec *= Vector3.Dot(heading, velocity.normalized);
        rigidbody.AddRelativeTorque(rotationVec, ForceMode.VelocityChange);

        //  Damping effect
        // ----------------------------------------
        var inertiaRotation = Quaternion.Lerp(
            rigidbody.rotation,
            Quaternion.LookRotation(heading, transform.up),
            speedQ * 4f * Time.deltaTime);
        rigidbody.MoveRotation(inertiaRotation);

        //  Turning effect
        // ----------------------------------------
        rigidbody.velocity = Vector3.SlerpUnclamped(
            rigidbody.velocity,
            transform.forward * rigidbody.velocity.magnitude,
            speedStallQ * 2f * Time.deltaTime);

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
        rigidbody.drag = dragNominal
            + brakesKnob.value * brakesDrag
            + turnDrag;
        rigidbody.angularDrag = 2 * speedQ + 2f;

        // Debug.Log(turnDrag);
    }

    public float GetSpeed() {
        return rigidbody.velocity.magnitude;
    }

    public float GetPitchDegrees() {
        var pitch = transform.rotation.eulerAngles.x;
        return Mathf.PingPong(pitch, 180) * (pitch < 180 ? -1 : 1);
    }

    public void OnGUI() {
        GUI.Label(new Rect(10, 10, 800, 20), "Speed: " + Util.Kph(GetSpeed()));
        GUI.Label(new Rect(10, 30, 800, 20), "Drag: " + rigidbody.drag);
        GUI.Label(new Rect(10, 50, 800, 20), "Thrust: " + thrustKnob.value);
        GUI.Label(new Rect(10, 70, 800, 20), "Brakes: " + brakesKnob.value);
        GUI.Label(new Rect(10, 90, 800, 20), "Altitude: " + transform.position.y);
        GUI.Label(new Rect(10, 110, 800, 20), "Pitch: " + GetPitchDegrees());
    }

    public void ShowDebugInfo() {
        var cameraTfm = GameObject
            .Find("FirstPersonCamera")
            .GetComponent<Camera>()
            .transform;
        Debug.DrawLine(cameraTfm.position + cameraTfm.forward,
            cameraTfm.position + rigidbody.velocity);
    }

}
