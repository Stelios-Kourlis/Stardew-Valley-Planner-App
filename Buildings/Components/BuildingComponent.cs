using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuildingData;

public abstract class BuildingComponent : MonoBehaviour {
    protected Building Building => gameObject.GetComponent<Building>();
    public abstract ComponentData Save();
    public abstract void Load(ComponentData data);
    // public int loadPriority = 0;
}
