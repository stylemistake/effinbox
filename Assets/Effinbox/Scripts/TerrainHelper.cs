using UnityEngine;

namespace Effinbox {

  [ExecuteInEditMode]
  [RequireComponent(typeof (Terrain))]
  public class TerrainHelper: MonoBehaviour {

    [RangeAttribute(0f, 20000f)]
    public float baseMapDistance;

    private Terrain terrain;

    private void Start() {
      terrain = GetComponent<Terrain>();
    }

    private void LateUpdate() {
      terrain.basemapDistance = baseMapDistance;
    }

  }

}
