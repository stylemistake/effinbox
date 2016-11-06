using UnityEngine;

public class Missile : MonoBehaviour {

    public Vector3 heading = Vector3.forward;
    public float speed = 600;
    public float maneuverability = 1;
    public float damage = 100;
    public float range = 2000;
    public GameObject target;

    private Rigidbody rigidbody;

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
        var speedA = speed;
        var speedB = velB.magnitude;
        var posDiff = target.transform.position - transform.position;
        var velQ = (velA.normalized + velB.normalized).magnitude;
        var posQ = (velA.normalized + posDiff.normalized).magnitude;
        Color color = Color.red;
        Vector3 targetHeading;
        if (speedA <= speedB || velQ < 0.1) {
            // No collision point
            // Find some middle-ground approach
            if (velQ > 1.8 && posQ < 0.2) {
                // Going ahead of the plane
                // Try with less speed, so collision happens faster
                color = Color.blue;
                targetHeading = (posDiff + velB).normalized
                    * speedA / 4;
            } else {
                color = Color.yellow;
                targetHeading = (posDiff + velB).normalized
                    * speedA;
            }
        } else {
            // Collision point exists
            // Find exact collision course
            targetHeading = posDiff.normalized
                * Mathf.Sqrt(speedA * speedA - speedB * speedB)
                + velB;
            // Try less speed to turn around faster
            if (velQ < 1.8 && posQ < 1.4) {
                color = Color.blue;
                targetHeading /= 4;
            }
        }
        var velDelta = maneuverability * speed / rigidbody.velocity.magnitude;
        rigidbody.velocity = Vector3.RotateTowards(
            rigidbody.velocity,
            targetHeading,
            Time.deltaTime * velDelta,
            Time.deltaTime * 800);
        Debug.DrawLine(rigidbody.worldCenterOfMass,
            rigidbody.worldCenterOfMass + (rigidbody.velocity / 4),
            color);
    }

    void OnCollisionEnter(Collision col) {
        Destroy(gameObject);
    }

}
