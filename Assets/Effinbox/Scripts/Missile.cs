using UnityEngine;

namespace Effinbox {

  [RequireComponent (typeof (Rigidbody))]
  public class Missile: MonoBehaviour {

    public Vector3 heading = Vector3.forward;

    [RangeAttribute(200f, 2000f)]
    public float speed = 600f;

    [RangeAttribute(0.2f, 4f)]
    public float maneuverability = 1f;

    [RangeAttribute(0f, 500f)]
    public float damage = 75f;

    [RangeAttribute(0.1f, 200f)]
    public float damageRange = 10f;

    [RangeAttribute(1000f, 20000f)]
    public float range = 2000f;

    public GameObject target;

    private float distanceTravelled;
    private new Rigidbody rigidbody;
    private bool hasCollided;

    // Use this for initialization
    private void Start() {
      rigidbody = GetComponent<Rigidbody>();
      rigidbody.velocity = heading * speed;
    }

    // Update is called once per frame
    private void FixedUpdate() {
      ApplyHoming();
      ApplyDragRotation();

      // Range limit
      distanceTravelled += rigidbody.velocity.magnitude * Time.deltaTime;
      if (distanceTravelled > range) {
        Destroy(gameObject);
      }
    }

    private void ApplyDragRotation() {
      var velocity = rigidbody.velocity;
      var rotation = Quaternion.LookRotation(velocity);
      rigidbody.MoveRotation(rotation);
    }

    private void ApplyHoming() {
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

    private void OnCollisionEnter() {
      if (hasCollided) {
        return;
      }
      var exploder = GetComponent<ExplodeOnDestroy>();
      if (exploder) {
        exploder.Enable();
      }
      // Calculate damage
      if (target) {
        var health = target.GetComponent<Health>();
        if (health) {
          var distance = Vector3.Distance(transform.position, target.transform.position);
          var distanceQ = Mathf.Clamp01((damageRange - distance) / damageRange);
          health.ApplyDamage(damage * distanceQ);
        }
      }
      hasCollided = true;
      Destroy(gameObject);
    }

  }

}
