using UnityEngine;

public class Gizmo: MonoBehaviour {

    public Color color = Color.white;
    public float radius = 1f;

    public void OnDrawGizmos() {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
