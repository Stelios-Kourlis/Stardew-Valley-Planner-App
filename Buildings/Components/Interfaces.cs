using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary> The base class that all building need to implement </summary>
// public interface IBuilding {
//     string BuildingName { get; }
//     Sprite Sprite { get; }
//     Vector3Int[] SpriteCoordinates { get; }
//     Vector3Int[] BaseCoordinates { get; }
//     bool IsPlaced { get; }
//     int Height { get; }
//     int Width { get; }
//     int BaseHeight { get; }
//     Tilemap Tilemap { get; }
//     TilemapRenderer TilemapRenderer { get; }
//     Transform Transform { get; }
//     GameObject BuildingGameObject { get; }
//     public (bool, BuildingData) IsPickedUp { get; }
//     bool PlaceBuilding(Vector3Int position);
//     bool PickupBuilding();
//     void DeleteBuilding(bool force = false);
//     void PlaceBuildingPreview(Vector3Int position);
//     void PickupBuildingPreview();
//     void DeleteBuildingPreview();
//     List<MaterialCostEntry> GetMaterialsNeeded();
//     BuildingData GetBuildingData();
//     bool LoadBuildingFromData(BuildingData data);
//     void UpdateTexture(Sprite sprite);
//     GameObject CreateBuildingButton();
//     Action BuildingPlaced { get; set; }
//     Action BuildingRemoved { get; set; }
//     public bool CanBeMassPlaced { get; }
// }

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
    // int GetConnectingFlags(bool includeTop = true);
    void UpdateSelf();
}
