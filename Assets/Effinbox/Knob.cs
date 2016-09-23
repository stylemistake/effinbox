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
        var maxDelta = this.rate * Time.deltaTime;
        var target = Mathf.Clamp(amount, this.min, this.max);
        this.value = Mathf.MoveTowards(this.value, target, maxDelta);
    }

    public void ApplyMax() {
        this.Apply(this.max);
    }

    public void ApplyMin() {
        this.Apply(this.min);
    }

    public void ApplyNominal() {
        this.Apply(this.nominal);
    }

}
