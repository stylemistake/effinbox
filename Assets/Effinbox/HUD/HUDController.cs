using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HUDController: MonoBehaviour {

    public GameObject player;

    private Rect screen;
    private Rigidbody playerRigidbody;
    // private AircraftPhysics playerAircraft;
    private Text speedText;
    private RectTransform speedSpinner;
    private Text pitchTextLeft;
    private Text pitchTextRight;
    private RectTransform velocityPointer;

    public void Start() {
        screen = GetComponent<RectTransform>().rect;
        playerRigidbody = player.GetComponent<Rigidbody>();
        // playerAircraft = player.GetComponent<AircraftPhysics>();
        speedText = transform.FindChild("SpeedText").GetComponent<Text>();
        speedSpinner = transform.FindChild("SpeedSpinner").GetComponent<RectTransform>();
        pitchTextLeft = transform.FindChild("PitchTextLeft").GetComponent<Text>();
        pitchTextRight = transform.FindChild("PitchTextRight").GetComponent<Text>();
        velocityPointer = transform.FindChild("VelocityPointer").GetComponent<RectTransform>();
    }

    public float GetPitchDegrees() {
        var pitch = player.transform.rotation.eulerAngles.x;
        return Mathf.PingPong(pitch, 180) * (pitch < 180 ? -1 : 1);
    }

    public Vector2 GetVelocityAxisAngles() {
        if (playerRigidbody.velocity.magnitude <= 0) {
            return Vector2.zero;
        }
        var up = player.transform.up;
        var forward = player.transform.forward;
        var heading = playerRigidbody.velocity.normalized;
        var rotation = Quaternion.Inverse(Quaternion.LookRotation(forward, up))
            * Quaternion.LookRotation(heading, up);
        var angles = rotation.eulerAngles;
        return new Vector2(
            Mathf.PingPong(angles.y, 180) * (angles.y < 180 ? 1 : -1),
            Mathf.PingPong(angles.x, 180) * (angles.x < 180 ? -1 : 1)) / 180;
    }

    public void LateUpdate() {
        var speed = playerRigidbody.velocity.magnitude;
        if (Application.isPlaying) {
            speedText.text = Util.Kph(speed).ToString("0");
        }
        speedSpinner.rotation = Quaternion.Euler(0f, 0f, -speed * 45);

        var pitchDegrees = GetPitchDegrees();
        pitchTextRight.text = pitchTextLeft.text = pitchDegrees.ToString("00");

        var velocityAngles = GetVelocityAxisAngles() * screen.width * 2;
        velocityPointer.localPosition = velocityAngles;
    }

}
