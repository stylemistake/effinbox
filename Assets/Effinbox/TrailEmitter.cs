using UnityEngine;
using System.Collections;

public class TrailEmitter : MonoBehaviour {

    public GameObject trailPrototype;
    private GameObject trail;

	// Use this for initialization
	void Start () {
        trail = Instantiate(trailPrototype);
	}

	// Update is called once per frame
	void Update () {
        trail.transform.position = transform.position;
	}

}
