using UnityEngine;

[ExecuteInEditMode]
public class AircraftChaseCamera : MonoBehaviour {

	public AircraftPhysics target;
    private new Transform camera;

    // Use this for initialization
    public void Start() {
		camera = GetComponent<Transform>();
	}

    // Update is called once per frame
    public void LateUpdate() {
		var targetPos = target.transform.position;
		var targetVel = target.GetComponent<Rigidbody>().velocity;
		var targetTfm = target.transform;
		var cameraOffset = - targetVel.normalized * 0 - targetTfm.forward * 25;
        camera.position = targetPos + cameraOffset;
		camera.rotation = Quaternion.LookRotation(targetTfm.forward, targetTfm.up);
		transform.Translate(Vector3.up * 8);
	}

}
