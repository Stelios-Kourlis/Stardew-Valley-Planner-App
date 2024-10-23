using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShippingBinBehaviour : BuildingBehaviourExtension {
    private bool isFirst;
    private static int amountOfShippingBins;

    public override void NoPreview() {
        return;
    }

    public override void OnDelete() {
        amountOfShippingBins--;
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
        amountOfShippingBins--;
    }

    public override void OnPickupPreview() {
        return;
    }

    public override void OnPlace(Vector3Int lowerLeftCorner) {
        amountOfShippingBins++;
        isFirst = amountOfShippingBins == 1;
    }

    public override void OnPlacePreview(Vector3Int lowerLeftCorner) {
        return;
    }

    public override void OnStart(Building building) {
        amountOfShippingBins = 0;
    }

    public override bool DiffrentMaterialCost(out List<MaterialCostEntry> alternativeMaterials) {

        alternativeMaterials = new();
        if (!isFirst) return false;
        alternativeMaterials = new(){
            new MaterialCostEntry("Free")
        };
        return true;
    }
}
