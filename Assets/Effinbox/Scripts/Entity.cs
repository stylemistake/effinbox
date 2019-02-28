using UnityEngine;

namespace Effinbox {

  public class Entity: MonoBehaviour {

    public string hudAbbr;
    public Faction faction;
    public EntityType type;
    public EntityPriority priority;
    public Squadron squadron;
    public bool isTargetable;
    public bool isRadarLockable;

  }

}
