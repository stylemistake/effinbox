using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class AircraftChaseCamera : MonoBehaviour {

	public AircraftPhysics target;
    private new Transform camera;

    // Use this for initialization
    public void Start() {
		this.camera = this.GetComponent<Transform>();
	}

    // Update is called once per frame
    public void LateUpdate() {
		var targetPos = this.target.transform.position;
		var targetVel = this.target.velocity;
		var targetTfm = this.target.transform;
		// var cameraOffset = - targetVel.normalized * 5 - targetTfm.forward * 15;
		var cameraOffset = - targetVel.normalized * 2 - targetTfm.forward * 23;
        this.camera.position = targetPos + cameraOffset;
		this.camera.rotation = Quaternion.LookRotation(targetTfm.forward, targetTfm.up);
		this.transform.Translate(Vector3.up * 8);
	}

}
