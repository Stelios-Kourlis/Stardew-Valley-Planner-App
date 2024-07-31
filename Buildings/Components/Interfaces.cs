using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary> This is implemented if a building needs to do extra actions on Place,Edit,Delete </summary>
public interface IExtraActionBuilding {
    void PerformExtraActionsOnPlace(Vector3Int position) { return; }
    void PerformExtraActionsOnPlacePreview(Vector3Int position) { return; }
    void PerformExtraActionsOnDelete() { return; }
    void PerformExtraActionsOnDeletePreview() { return; }
    void PerformExtraActionsOnPickup() { return; }
    void PerformExtraActionsOnPickupPreview() { return; }
    string[] GetExtraData() { return null; }
    void LoadExtraBuildingData(string[] data) { return; }
    string AddBeforeBuildingName() { return ""; }
    string AddToBuildingName() { return ""; }

}

public interface IConnectingBuilding {
    void UpdateSelf();
}
