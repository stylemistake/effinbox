using UnityEngine;

namespace Effinbox {

  public class TrailEmitter: MonoBehaviour {

    public GameObject trailPrototype;
    private GameObject trail;

    // Use this for initialization
    public void Start() {
      trail = Instantiate(trailPrototype);
    }

    // Update is called once per frame
    public void Update() {
      trail.transform.position = transform.position;
    }

  }

}
