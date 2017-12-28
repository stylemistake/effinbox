using UnityEngine;

namespace Effinbox {

  public class Health: MonoBehaviour {

    [RangeAttribute(0f, 500f)]
    public float health = 100f;

    public void ApplyDamage(float damage) {
        if (damage > 0) {
            health -= damage;
            Debug.Log("Damage: " + damage.ToString("0"));
        }
    }

  }

}
