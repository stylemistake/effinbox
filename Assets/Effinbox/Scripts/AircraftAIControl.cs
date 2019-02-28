using UnityEngine;

namespace Effinbox {

  [RequireComponent (typeof (AircraftPhysics))]
  [RequireComponent (typeof (Rigidbody))]
  public class AircraftAIControl: MonoBehaviour {

    public Waypoint initialWaypoint;
    public Entity leader;
    public Entity target;

    [RangeAttribute(0f, 90f)]
    public float deadzoneAngle = 45f;

    [RangeAttribute(0f, 5f)]
    public float deadzoneSize = 3f;

    [RangeAttribute(0f, 1000f)]
    public float minAltitude = 100f;

    private AircraftPhysics aircraft;
    private AircraftWeapon aircraftWeapon;
    private new Rigidbody rigidbody;
    private Radar radar;
    private Waypoint waypoint;
    private float lastFiredAt;

    public void Start() {
      aircraft = GetComponent<AircraftPhysics>();
      aircraftWeapon = GetComponent<AircraftWeapon>();
      rigidbody = GetComponent<Rigidbody>();
      radar = GetComponent<Radar>();
      waypoint = initialWaypoint;
    }

    public void FixedUpdate() {
      if (radar) {
        radar.SelectNearestEnemy();
        target = radar.GetSelectedTarget();
      }
      if (target) {
        AlignToSelectedTarget();
        var nextFireAt = lastFiredAt + 0.25f;
        if (aircraftWeapon && radar.IsLocked() && nextFireAt < Time.time) {
          aircraftWeapon.FireWeapon();
          lastFiredAt = Time.time;
        }
      }
      else if (waypoint) {
        if (IsPositionReached(waypoint.position)) {
          waypoint = waypoint.GetNearest();
        }
        if (IsPositionReachable(waypoint.position)) {
          AlignToWaypoint();
        }
        else {
          AlignToHorizon();
        }
      }
      else {
        AlignToHorizon();
      }
      if (rigidbody.velocity.magnitude < aircraft.speedStall) {
        aircraft.ApplySpeedControl(1f, 0f);
      }
      if (rigidbody.velocity.magnitude > aircraft.speedNominal) {
        aircraft.ApplySpeedControl(0f, 0f);
      }
    }

    public float GetTerrainAltitude() {
      var pos = transform.position + rigidbody.velocity * deadzoneSize;
      return pos.y - Terrain.activeTerrain.SampleHeight(pos);
    }

    public bool IsPositionReached(Vector3 position) {
      var distance = Vector3.Distance(transform.position, position);
      return distance <= rigidbody.velocity.magnitude;
    }

    public bool IsPositionReachable(Vector3 position) {
      return !Util.InDeadzoneCone(transform, position, deadzoneAngle,
        rigidbody.velocity.magnitude * deadzoneSize);
    }

    public void AlignToHorizon() {
      var altitude = GetTerrainAltitude();
      var horizontal = transform.forward;
      horizontal.y = altitude < minAltitude ? 0.5f : 0;
      var rotationAngles = Util.AxisAnglesAlt(transform, horizontal, Vector3.up) / 90;
      rotationAngles.x *= 8f;
      rotationAngles.y *= 2f;
      rotationAngles.z *= -1f;
      aircraft.ApplyHeadingControl(rotationAngles);
      aircraft.ApplySpeedControl(1f, 0f);
    }

    public void AlignToWaypoint() {
      AlignToWorldPoint(waypoint.position);
    }

    public void AlignToSelectedTarget() {
      AlignToWorldPoint(target.transform.position);
    }

    public void AlignToWorldPoint(Vector3 position) {
      var heading = position - transform.position;

      // Keep a safe altitude
      var altitude = GetTerrainAltitude();
      var distance = heading.magnitude;
      if (distance > minAltitude && altitude < minAltitude) {
        AlignToHorizon();
        return;
      }

      // Angles relative to the plane
      var angles = Util.AxisAnglesAlt(transform, heading, Vector3.up) / 90;
      // Angles relative to the world
      var anglesWorld = Util.AxisAnglesAlt(transform.forward, Vector3.up, heading) / 90;
      // Bank angle
      var bank = Util.AxisAnglesAlt(transform, transform.forward, Vector3.up).z;
      // Roll amount to keep plane vertical to pitch towards the target
      var rollVertical = Mathf.DeltaAngle(bank, anglesWorld.y > 0 ? 85 : -85) / 90;

      // if (Util.InRangeAbs(angles.y, 1f, 2f)) {
      //   // Away from the target
      // } else {
      //   // Towards the target
      // }

      aircraft.ApplySpeedControl(0.6f, 0f);

      if (Util.InRangeAbs(anglesWorld.y, 0f, 0.15f)) {
        // Stabilizing
        var rotationAngles = angles;
        rotationAngles.x *= 5f;
        rotationAngles.y *= 10f;
        rotationAngles.z *= -1f;
        aircraft.ApplyHeadingControl(rotationAngles);
      } else {
        // Pitching
        aircraft.ApplyRoll(rollVertical - anglesWorld.x * 2f);
        aircraft.ApplyPitch(angles.x * 5f);
        aircraft.ApplyYaw(0f);
      }
    }

    public void OnDrawGizmos() {
      Vector3? targetPos = null;
      if (target) {
        targetPos = target.transform.position;
      }
      else if (waypoint) {
        targetPos = waypoint.position;
      }
      if (targetPos.HasValue) {
        Gizmos.color = IsPositionReachable(targetPos.Value)
          ? Color.red
          : Color.yellow;
        Gizmos.DrawLine(transform.position, targetPos.Value);
      }
    }

  }

}
