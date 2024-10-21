using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BuildingBehaviourExtension {
    protected Building Building;
    public abstract void OnStart(Building building);
    public abstract void OnPlacePreview(Vector3Int lowerLeftCorner);
    public abstract void OnPlace(Vector3Int lowerLeftCorner);
    public abstract void OnPickupPreview();
    public abstract void OnPickup();
    public abstract void OnDeletePreview();
    public abstract void OnDelete();
    public abstract void NoPreview();
    public abstract void OnMouseEnter();
    public abstract void OnMouseExit();
    public abstract void OnDestroy();
    public virtual bool BuildingSpecificPlacementPreconditionsAreMet(Vector3Int position, out string errorMessage) { errorMessage = ""; return true; }
    public virtual bool DiffrentMaterialCost(out List<MaterialCostEntry> alternativeMaterials) { alternativeMaterials = new(); return false; }
}
