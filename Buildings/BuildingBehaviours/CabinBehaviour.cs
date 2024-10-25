using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CabinBehaviour : BuildingBehaviourExtension {
    static Dictionary<string, bool> cabinsPlaced;
    MultipleTypeBuildingComponent multipleTypeBuildingComponent;
    public override void NoPreview() {
        return;
    }

    public override void OnDelete() {
        cabinsPlaced[multipleTypeBuildingComponent.CurrentTypeRaw] = false;
    }

    public override void OnDeletePreview() {
        return;
    }

    public override void OnDestroy() {
        return;
    }

    public override void OnMouseEnter() {
        return;
    }

    public override void OnMouseExit() {
        return;
    }

    public override void OnPickup() {
        cabinsPlaced[multipleTypeBuildingComponent.CurrentTypeRaw] = false;
    }

    public override void OnPickupPreview() {
        return;
    }

    public override void OnPlace(Vector3Int lowerLeftCorner) {
        cabinsPlaced[multipleTypeBuildingComponent.CurrentTypeRaw] = true;
    }

    public override void OnPlacePreview(Vector3Int lowerLeftCorner) {
        return;
    }

    public override void OnStart(Building building) {
        Building = building;
        multipleTypeBuildingComponent = building.gameObject.GetComponent<MultipleTypeBuildingComponent>();
        if (cabinsPlaced != null) return;
        cabinsPlaced = new();
        foreach (BuildingVariant variant in multipleTypeBuildingComponent.variants) {
            cabinsPlaced.Add(variant.variantName, false);
        }
    }

    public override bool BuildingSpecificPlacementPreconditionsAreMet(Vector3Int position, out string errorMessage) {
        errorMessage = "";
        bool hasThisCabinTypeBeenPlaced = cabinsPlaced[multipleTypeBuildingComponent.CurrentTypeRaw];
        if (hasThisCabinTypeBeenPlaced) {
            errorMessage = "You can only have one of each type of cabin";
            return false;
        }
        return true;
    }
}
