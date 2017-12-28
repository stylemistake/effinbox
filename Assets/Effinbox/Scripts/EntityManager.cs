using UnityEngine;
using System.Collections.Generic;

namespace Effinbox {

  [ExecuteInEditMode]
  public class EntityManager: MonoBehaviour {

    private readonly List<Entity> entities = new List<Entity>();

    public void Start() {
      InvokeRepeating("ReloadEntities", 0f, 0.25f);
    }

    public void ReloadEntities() {
      var entityArray = transform.GetComponentsInChildren<Entity>();
      entities.Clear();
      entities.AddRange(entityArray);
    }

    public List<Entity> GetAll() {
      return entities;
    }

    public List<Entity> GetTargets(Entity self) {
      return entities.FindAll(x => x != self && x.isTargetable);
    }

  }

}
