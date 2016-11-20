using UnityEngine;

[ExecuteInEditMode]
public class AircraftChaseCamera : MonoBehaviour {

    public AircraftPhysics target;
    private new Transform camera;

    public float height;
    public float distance;

    // Use this for initialization
    public void Start() {
        camera = GetComponent<Transform>();
    }

    // Update is called once per frame
    public void LateUpdate() {
        var targetPos = target.transform.position;
        var targetVel = target.GetComponent<Rigidbody>().velocity;
        var targetTfm = target.transform;
        var cameraOffset = - targetTfm.forward * distance;
        if (targetVel.magnitude > 1) {
            cameraOffset -= targetVel.normalized * distance / 10;
        } else {
            cameraOffset -= targetTfm.forward * distance / 10;
        }
        camera.position = targetPos + cameraOffset;
        camera.rotation = Quaternion.LookRotation(targetTfm.forward, targetTfm.up);
        transform.Translate(Vector3.up * height);
    }

}
