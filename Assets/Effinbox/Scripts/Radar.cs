using UnityEngine;
using System.Collections.Generic;

namespace Effinbox {

  public class Radar: MonoBehaviour {

    public EntityManager entityMgr;
    public float lockFOV;
    public float lockRange;

    private Entity self;
    private Entity target;

    public void Start() {
      self = GetComponent<Entity>();
    }

    public void LateUpdate() {
      if (!target) {
        target = GetNearestEnemy();
      }
    }

    public Entity GetSelectedTarget() {
      return target;
    }

    public void SetSelectedTarget(Entity target) {
      this.target = target;
    }

    public List<Entity> GetTargets() {
      return entityMgr.GetTargets(self);
    }

    public List<Entity> GetEnemies() {
      return GetTargets().FindAll(IsEnemy);
    }

    public Entity GetNearestEnemy() {
      var entities = GetEnemies();
      var nearestDistance = Mathf.Infinity;
      Entity nearest = null;
      foreach (var entity in entities) {
        var direction = Vector3.Normalize(transform.position
          - entity.transform.position);
        var cost = Mathf.Clamp01(Vector3.Dot(transform.forward, direction) + 1);
        var distance = cost * Vector3.Distance(self.transform.position,
          entity.transform.position);
        if (distance < nearestDistance) {
          nearest = entity;
          nearestDistance = distance;
        }
      }
      return nearest;
    }

    public Entity SelectNearestEnemy() {
      target = GetNearestEnemy();
      return target;
    }

    public void CycleTargets() {
      var targets = GetEnemies();
      if (target != null) {
        var nextIndex = targets.FindIndex(x => x == target) + 1;
        target = targets[nextIndex % targets.Count];
      } else {
        target = targets[0];
      }
    }

    public bool IsEnemy() {
      return target && target.faction != self.faction;
    }

    public bool IsEnemy(Entity entity) {
      return entity.faction != self.faction;
    }

    public bool IsSelected(Entity entity) {
      return target && entity == target;
    }

    public bool IsLocked() {
      return target && IsLocked(target);
    }

    public bool IsLocked(Entity entity) {
      if (!entity) {
        return false;
      }
      if (!IsSelected(entity)) {
        return false;
      }
      var localPosition = entity.transform.position - transform.position;
      var dot = Vector3.Dot(transform.forward, localPosition.normalized);
      return localPosition.magnitude < lockRange && dot > 1 - lockFOV / 90;
    }

  }

}
