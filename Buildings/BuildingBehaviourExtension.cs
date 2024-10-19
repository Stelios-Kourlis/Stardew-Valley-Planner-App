using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BuildingBehaviourExtension {
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
}
