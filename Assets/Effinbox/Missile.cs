using UnityEngine;

public class Missile : MonoBehaviour {

    public Vector3 heading = Vector3.forward;
    public float speed = 600;
    public float maneuverability = 1;
    public float damage = 100;
    public float range = 2000;
    public GameObject target;

    private float distanceTravelled = 0;
    private new Rigidbody rigidbody;

	// Use this for initialization
	void Start() {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = heading * speed;
	}

	// Update is called once per frame
	void FixedUpdate() {
        ApplyHoming();
        ApplyDragRotation();

        // Range limit
        distanceTravelled += rigidbody.velocity.magnitude * Time.deltaTime;
        if (distanceTravelled > range) {
            Destroy(gameObject);
        }
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
        var targetRigidbody = target.GetComponent<Rigidbody>();
        var velA = rigidbody.velocity;
        Vector3 velB = Vector3.zero;
        if (targetRigidbody != null) {
            velB = targetRigidbody.velocity;
        }
        var speedA = speed;
        var speedB = velB.magnitude;
        var posDiff = target.transform.position - transform.position;
        var velQ = (velA.normalized + velB.normalized).magnitude;
        // var posQ = (velA.normalized + posDiff.normalized).magnitude;
        Color color = Color.red;
        Vector3 targetHeading;
        if (speedA <= speedB || velQ < 0.1) {
            // No collision point
            // Find some middle-ground approach
            color = Color.yellow;
            targetHeading = (posDiff + velB).normalized
                * speedA;
        } else {
            // Collision point exists
            // Find exact collision course
            targetHeading = posDiff.normalized
                * Mathf.Sqrt(speedA * speedA - speedB * speedB)
                + velB;
        }
        rigidbody.velocity = Vector3.RotateTowards(
            rigidbody.velocity,
            targetHeading,
            Time.deltaTime * maneuverability,
            Time.deltaTime * 1000);
        Debug.DrawLine(rigidbody.worldCenterOfMass,
            rigidbody.worldCenterOfMass + (rigidbody.velocity / 4),
            color);
    }

    void OnCollisionEnter(Collision col) {
        Destroy(gameObject);
    }

}
