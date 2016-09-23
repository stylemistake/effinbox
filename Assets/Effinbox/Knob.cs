using UnityEngine;

public class Knob {

    public float value;
    public float rate;
    public float min;
    public float max;
    public float nominal;

    public static Knob CreateBipolar(float rate) {
        return new Knob(0.0f, rate, -1.0f, 1.0f);
    }

    public Knob(float value, float rate, float min, float max) {
        this.value = value;
        this.rate = rate;
        this.min = min;
        this.max = max;
        this.nominal = value;
    }

    public Knob(float value, float rate, float min, float max, float nominal) {
        this.value = value;
        this.rate = rate;
        this.min = min;
        this.max = max;
        this.nominal = nominal;
    }

    public void Apply(float amount) {
        var target = Mathf.Clamp(amount, min, max);
        value = Mathf.MoveTowards(value, target, rate * Time.deltaTime);
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
