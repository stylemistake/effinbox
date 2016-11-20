using UnityEngine;

public class ParticleSpeedOpacity: MonoBehaviour {

    public float minSpeed = 50f;
    public float maxSpeed = 250f;
    public new Rigidbody rigidbody;

    private ParticleSystem particles;
    private Color color = new Color(1f, 1f, 1f, 0f);

    public void Start() {
        particles = GetComponent<ParticleSystem>();
    }

    public void LateUpdate() {
        color.a = Util.RelativeRangeValue(
            rigidbody.velocity.magnitude,
            minSpeed, maxSpeed);
        particles.startColor = color;
    }

}
