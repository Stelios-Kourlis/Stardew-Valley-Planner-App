using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassPlaceableBuildingComponent : MonoBehaviour {
    private IMassPlaceableBuilding Building => gameObject.GetComponent<IMassPlaceableBuilding>();

    // public void MassPlace(Vector3Int[] positions) {
    //     foreach (Vector3Int position in positions) {
    //         Building.PlaceBuilding(position);
    //     }
    // }

    // public void MassPlacePreview(Vector3Int[] positions) {

    // }
}
