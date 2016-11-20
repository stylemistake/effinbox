using UnityEngine;

public class Knob {

    public float value;
    public float rate;
    public float min;
    public float max;
    public float nominal;
    public bool linear = false;

    public float magnitude {
        get {
            return Mathf.Abs(value);
        }
    }

    public static Knob CreateBipolar(float rate) {
        return new Knob(0.0f, -1.0f, 1.0f, rate);
    }

    public Knob(float value, float min, float max, float rate) {
        this.value = value;
        this.rate = rate;
        this.min = min;
        this.max = max;
        this.nominal = value;
    }

    public Knob(float value, float min, float max, float rate, float nominal) {
        this.value = value;
        this.rate = rate;
        this.min = min;
        this.max = max;
        this.nominal = nominal;
    }

    public void Apply(float amount) {
        var target = Mathf.Clamp(amount, min, max);
        value = linear
            ? Mathf.MoveTowards(value, target, rate * Time.deltaTime)
            : Mathf.Lerp(value, target, rate * Time.deltaTime * 2);
    }

    public void ApplyMax() {
        Apply(max);
    }

    public void ApplyMin() {
        Apply(min);
    }

    public void ApplyNominal() {
        Apply(nominal);
    }

}
