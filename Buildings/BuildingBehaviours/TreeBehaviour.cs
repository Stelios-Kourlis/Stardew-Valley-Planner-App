using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : BuildingBehaviourExtension {
    public override void NoPreview() {
        return;
    }

    public override void OnDelete() {
        return;
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
        return;
    }

    public override void OnPickupPreview() {
        return;
    }

    public override void OnPlace(Vector3Int lowerLeftCorner) {
        // InvalidTilesManager.
        return;
    }

    public override void OnPlacePreview(Vector3Int lowerLeftCorner) {
        return;
    }

    public override void OnStart(Building building) {
        return;
    }
}
