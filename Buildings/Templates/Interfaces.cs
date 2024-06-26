using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary> The base class that all building need to implement </summary>
public interface IBuilding {
    string BuildingName { get; }
    int UID { get; }
    Sprite Sprite { get; }
    Vector3Int[] SpriteCoordinates { get; }
    Vector3Int[] BaseCoordinates { get; }
    bool IsPlaced { get; }
    int Height { get; }
    int Width { get; }
    int BaseHeight { get; }
    Tilemap Tilemap { get; }
    TilemapRenderer TilemapRenderer { get; }
    Transform Transform { get; }
    GameObject BuildingGameObject { get; }
    public (bool, string) IsPickedUp { get; }
    bool PlaceBuilding(Vector3Int position);
    bool PickupBuilding();
    void DeleteBuilding(bool force = false);
    void PlaceBuildingPreview(Vector3Int position);
    void PickupBuildingPreview();
    void DeleteBuildingPreview();
    List<MaterialInfo> GetMaterialsNeeded();
    string GetBuildingData();
    bool LoadBuildingFromData(string[] data);
    void UpdateTexture(Sprite sprite);
    GameObject CreateBuildingButton();
    Action BuildingPlaced { get; set; }
    Action BuildingRemoved { get; set; }
    // public bool CanBeMassPlaced { get; }
}

/// <summary> This is implemented if a building needs to do extra actions on Place,Edit,Delete </summary>
public interface IExtraActionBuilding {
    void PerformExtraActionsOnPlace(Vector3Int position) { return; }
    void PerformExtraActionsOnPlacePreview(Vector3Int position) { return; }
    void PerformExtraActionsOnDelete() { return; }
    void PerformExtraActionsOnDeletePreview() { return; }
    void PerformExtraActionsOnPickup() { return; }
    void PerformExtraActionsOnPickupPreview() { return; }
    string AddToBuildingData() { return ""; }
    void LoadExtraBuildingData(string[] data) { return; }
    string AddBeforeBuildingName() { return ""; }
    string AddToBuildingName() { return ""; }

}

/// <summary> This is implemented if a building can be interacted with </summary>
public interface IInteractableBuilding : IBuilding, IExtraActionBuilding {
    List<ButtonTypes> BuildingInteractions { get; }
    GameObject ButtonParentGameObject { get; }
    void OnMouseRightClick() { return; }
    void OnMouseEnter() { return; }
    void OnMouseExit() { return; }
}

public interface ITieredBuilding : IInteractableBuilding {
    int Tier { get; }
    int MaxTier { get; }
    void SetTier(int tier);
    // List<ButtonTypes> IInteractableBuilding.BuildingInteractions => IBuilding.BuildingGameObject.GetComponent<InteractableBuildingComponent>().BuildingInteractions;
}

public interface IMultipleTypeBuilding : IBuilding, IInteractableBuilding { //its not really interactable but is needed for sprite names
    Enum Type { get; }
    void CycleType();
    void SetType(Enum type);
    GameObject[] CreateButtonsForAllTypes();
}

public interface IAnimalHouse : IInteractableBuilding {
    List<KeyValuePair<Animals, GameObject>> AnimalsInBuilding { get; }
    bool AddAnimal(Animals animal);
    void ToggleAnimalMenu();
}

public interface IRangeEffectBuilding : IBuilding {
    void ShowEffectRange(Vector3Int[] RangeArea);
    void HideEffectRange();
}

public interface IConnectingBuilding : IInteractableBuilding {
    int GetConnectingFlags(bool includeTop = true);
    void UpdateSelf();
}

public interface IEnterableBuilding : IInteractableBuilding {
    Vector3Int[] InteriorUnavailableCoordinates { get; }
    Vector3Int[] InteriorPlantableCoordinates { get; }
    void ToggleEditBuildingInterior();
    void EditBuildingInterior();
    void ExitBuildingInteriorEditing();
    void CreateInteriorCoordinates();

}

public interface IPaintableBuilding : IBuilding, IInteractableBuilding {
    //not implemented yet
}

public interface IMassPlaceableBuilding : IBuilding {
    void MassPlaceBuilding(Vector3Int[] positions);
    void MassPlaceBuildingPreview(Vector3Int[] positions);
}
