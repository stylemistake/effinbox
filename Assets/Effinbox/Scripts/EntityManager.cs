using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;

[ExecuteInEditMode]
public class EntityManager: MonoBehaviour {

    private List<Entity> entities = new List<Entity>();

    public void Start() {
        InvokeRepeating("ReloadEntities", 0f, 0.25f);
        Type type = Type.GetType("Mono.Runtime");
        if (type != null) {
            MethodInfo displayName = type.GetMethod("GetDisplayName",BindingFlags.NonPublic|BindingFlags.Static);
            if (displayName != null) {
                Debug.Log(displayName.Invoke(null,null));
            }
        }
    }

    public void ReloadEntities() {
        var entityArray = transform.GetComponentsInChildren<Entity>();
        entities.Clear();
        entities.AddRange(entityArray);
    }

    public List<Entity> GetTargets(Entity self) {
        return entities.FindAll(x => x.isTargetable);
    }

}
