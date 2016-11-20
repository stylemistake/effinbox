using UnityEngine;
using System.Collections.Generic;

public class Waypoint: MonoBehaviour {

    public List<Waypoint> linksTo;
    public Vector3 position {
        get {
            return transform.position;
        }
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(position, 40);
        foreach (var waypoint in linksTo) {
            if (!waypoint) {
                continue;
            }
            Gizmos.DrawLine(position, waypoint.position);
        }
    }

    public Waypoint GetNearest() {
        float maxDist = Mathf.Infinity;
        Waypoint result = null;
        foreach (var waypoint in linksTo) {
            if (!waypoint) {
                continue;
            }
            var dist = Vector3.Distance(position, waypoint.position);
            if (dist < maxDist) {
                maxDist = dist;
                result = waypoint;
            }
        }
        return result;
    }

}
