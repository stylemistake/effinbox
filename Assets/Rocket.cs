using UnityEngine;

public class Rocket : MonoBehaviour {

    public Vector3 heading = Vector3.forward;
    public float speed = 500;
    public GameObject target;
    public Rigidbody rigidbody;

	// Use this for initialization
	void Start() {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = heading * speed;
	}

	// Update is called once per frame
	void FixedUpdate() {
        ApplyHoming();
        ApplyDragRotation();
	}

    void ApplyDragRotation() {
        var velocity = rigidbody.velocity;
        var rotation = Quaternion.LookRotation(velocity);
        rigidbody.MoveRotation(rotation);
    }

    void ApplyHoming() {
        if (target == null) {
            return;
        }
        var velA = rigidbody.velocity;
        var velB = target.GetComponent<Rigidbody>().velocity;
        var speedA = velA.magnitude;
        var speedB = velB.magnitude;
        var positionDiff = target.transform.position - transform.position;
        var targetHeading = positionDiff.normalized * speed;
        // Mathf.Sqrt(speedA * speedA - speedB * speedB);
        rigidbody.velocity = Vector3.Lerp(heading, targetHeading, Time.deltaTime)
            .normalized * speed;
        Debug.DrawLine(rigidbody.worldCenterOfMass,
            rigidbody.worldCenterOfMass + rigidbody.velocity);
    }

    // GameObject FindClosestTarget() {
    //     GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
    //     GameObject closest = null;
    //     var distance = Mathf.Infinity;
    //     var position = transform.position;
    //     foreach (var target in targets) {
    //         var diff = target.transform.position - position;
    //         float curDistance = diff.sqrMagnitude;
    //         if (curDistance < distance) {
    //             closest = target;
    //             distance = curDistance;
    //         }
    //     }
    //     return closest;
    // }

    // void OnCollisionEnter(Collision col) {
    //     Debug.Log(col);
    // }

}
