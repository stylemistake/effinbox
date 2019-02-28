using UnityEngine;

namespace Effinbox {

  public class ExplodeOnDestroy: MonoBehaviour {

    public GameObject explosionPrototype;
    public bool explosionEnabled;

    public void Enable() {
      explosionEnabled = true;
    }

    public void Disable() {
      explosionEnabled = false;
    }

    public void OnDestroy() {
      if (explosionEnabled) {
        explosionPrototype.transform.position = transform.position;
        Instantiate(explosionPrototype);
      }
    }

  }

}
