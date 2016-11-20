using UnityEngine;

public class AircraftAIControl: MonoBehaviour {

    enum Maneuver {
        Rolling,
        Pitching,
        Stabilizing,
        Seeking,
        None,
    }

    public Waypoint initialWaypoint;
    public Entity leader;
    public float deadzoneAngle = 0.5f;
    public float deadzoneSize = 3f;
    public float minAltitude = 200f;

    private AircraftPhysics aircraft;
    private new Rigidbody rigidbody;
    private Waypoint waypoint;
    private Maneuver maneuver = Maneuver.None;

    public void Start() {
        aircraft = GetComponent<AircraftPhysics>();
        rigidbody = GetComponent<Rigidbody>();
        waypoint = initialWaypoint;
    }

    public void FixedUpdate() {
        if (waypoint) {
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
        // switch (maneuver) {
        //     case Maneuver.Rolling:
        //         Debug.Log("Rolling");
        //         break;
        //     case Maneuver.Pitching:
        //         Debug.Log("Pitching");
        //         break;
        //     case Maneuver.Stabilizing:
        //         Debug.Log("Stabilizing");
        //         break;
        //     case Maneuver.Seeking:
        //         Debug.Log("Seeking");
        //         break;
        //     default:
        //         Debug.Log("Doing nothing");
        //         break;
        // }
    }

    public void AlignToHorizon() {
        maneuver = Maneuver.Stabilizing;
        var altitude = GetTerrainAltitude();
        var horizontal = transform.forward;
        horizontal.y = altitude < minAltitude ? 0.5f : 0;
        var rotationAngles = Util.AxisAngles(transform, horizontal, Vector3.up);
        rotationAngles.x *= -5;
        rotationAngles.y *= -5;
        rotationAngles.z *= 2;
        aircraft.ApplyHeadingControl(rotationAngles);
    }

    public void AlignToTarget() {

    }

    public float GetTerrainAltitude() {
        var pos = transform.position;
        return pos.y - Terrain.activeTerrain.SampleHeight(pos);
    }

    public bool IsPositionReached(Vector3 position) {
        var distance = Vector3.Distance(transform.position, position);
        return distance <= rigidbody.velocity.magnitude;
    }

    public bool IsPositionReachable(Vector3 position) {
        var angle = Mathf.Abs(Vector3.Dot(transform.forward,
            (position - transform.position).normalized));
        var minDistance = rigidbody.velocity.magnitude * deadzoneSize;
        var distance = Vector3.Distance(transform.position, position);
        if (distance > minDistance) {
            return true;
        }
        if (distance < rigidbody.velocity.magnitude) {
            return false;
        }
        return angle > deadzoneAngle;
    }

    public void AlignToWaypoint() {
        var localWaypointPos = waypoint.position - transform.position;
        // Keep a safe altitude
        var altitude = GetTerrainAltitude();
        var distance = localWaypointPos.magnitude;
        if (distance > minAltitude && altitude < minAltitude) {
            AlignToHorizon();
            return;
        }
        // Continue with alignment
        var waypointAngles = Util.AxisAngles(transform, localWaypointPos, Vector3.up);
        var relativeAngles = Util.AxisAngles(transform, localWaypointPos);
        var roll = waypointAngles.x - waypointAngles.y;
        if (waypointAngles.y < 0) {
            roll = - waypointAngles.y - waypointAngles.x;
        }
        var rollAbs = Mathf.Abs(roll);
        if (maneuver == Maneuver.None || maneuver == Maneuver.Stabilizing) {
            maneuver = Maneuver.Seeking;
        }
        if (maneuver != Maneuver.Pitching && rollAbs > 0.2f) {
            maneuver = Maneuver.Rolling;
        }
        if (maneuver == Maneuver.Rolling) {
            aircraft.ApplyRoll(roll);
            aircraft.ApplyYaw(0);
            if (rollAbs < 0.75f) {
                maneuver = Maneuver.Pitching;
            }
        }
        if (maneuver == Maneuver.Pitching) {
            aircraft.ApplyPitch(-waypointAngles.x * 5);
            aircraft.ApplyYaw(0);
            aircraft.ApplyRoll(roll);
            aircraft.ApplySpeedControl(1f, 0f);
            if (Mathf.Abs(relativeAngles.y) < 0.5f && relativeAngles.x < 0.05f) {
                maneuver = Maneuver.Seeking;
            }
        }
        if (maneuver == Maneuver.Seeking) {
            var rotationAngles = Util.AxisAngles(transform, localWaypointPos, Vector3.up);
            rotationAngles.x *= -5;
            rotationAngles.y *= -10;
            rotationAngles.z *= 1;
            aircraft.ApplyHeadingControl(rotationAngles);
        }
    }

    public void OnDrawGizmos() {
        if (waypoint) {
            Gizmos.color = IsPositionReachable(waypoint.position)
                ? Color.red
                : Color.yellow;
            Gizmos.DrawLine(transform.position, waypoint.position);
        }
    }

}
