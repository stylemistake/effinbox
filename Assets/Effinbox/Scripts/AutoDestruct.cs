using UnityEngine;

namespace Effinbox {

  public class AutoDestruct: MonoBehaviour {

    public float time = 15f;

    public void Start() {
      Destroy(gameObject, time);
    }

  }

}
