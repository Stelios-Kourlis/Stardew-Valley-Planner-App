using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.SpriteManager;
using static Utility.TilemapManager;
using static Utility.BuildingManager;
using static Utility.ClassManager;
using System;
using UnityEngine.UI;

public static class BuildingController {
    /// <summary> A coordinate is unavailable if it is occupied by a building or if its out of bounds for the current map </summary>
    // private static readonly HashSet<Vector3Int> unavailableCoordinates = new();
    // private static readonly HashSet<Vector3Int> plantableCoordinates = new();
    public static SpecialCoordinatesCollection specialCoordinates = new();
    public static readonly List<Building> buildings = new();
    public static Type currentBuildingType = typeof(FishPond);
    public static Actions CurrentAction { get; private set; } = Actions.PLACE_WALLPAPER;
    public static bool IsLoadingSave { get; set; } = false;
    public static KeyValuePair<bool, EnterableBuildingComponent> isInsideBuilding = new(false, null);
    public static Transform CurrentTilemapTransform { get; private set; }

    public static HashSet<GameObject> buildingGameObjects = new();
    public static GameObject LastBuildingObjectCreated { get; private set; }
    public static Building LastBuildingCreated => LastBuildingObjectCreated != null ? LastBuildingObjectCreated.GetComponent<Building>() : null;
    public static Building CurrentBuildingBeingPlaced { get; set; }


    public static Action<Type> currentBuildingTypeChanged;
    public static Action anyBuildingPositionChanged;

    public static void CreateNewBuilding() {
        // Debug.Log("Creating new building");
        if (IsLoadingSave) return;
        Enum type = null;
        bool lastBuildingWasMultipleType = LastBuildingObjectCreated != null && LastBuildingObjectCreated.TryGetComponent(out MultipleTypeBuildingComponent _);
        if (lastBuildingWasMultipleType) {
            type = LastBuildingObjectCreated.GetComponent<MultipleTypeBuildingComponent>().Type;
        }
        LastBuildingObjectCreated = CreateNewBuildingGameObject(currentBuildingType);
        if (LastBuildingObjectCreated.TryGetComponent(out MultipleTypeBuildingComponent multipleTypeBuildingComponent)) {
            if (type != null) multipleTypeBuildingComponent.SetType(type);
        }
    }

    public static void DeleteCurrentBuilding() {
        if (CurrentBuildingBeingPlaced == null) return;
        CurrentBuildingBeingPlaced.DeleteBuilding();
    }

    // public static void AddToUnavailableCoordinates(Vector3Int[] coordinates) {
    //     if (!isInsideBuilding.Key) GetUnavailableCoordinates().UnionWith(coordinates);
    //     else isInsideBuilding.Value.InteriorUnavailableCoordinates.UnionWith(coordinates);
    //     InvalidTilesManager.Instance.UpdateUnavailableCoordinates();
    // }

    // public static void RemoveFromUnavailableCoordinates(Vector3Int[] coordinates) {
    //     if (!isInsideBuilding.Key) GetUnavailableCoordinates().ExceptWith(coordinates);
    //     else isInsideBuilding.Value.InteriorUnavailableCoordinates.ExceptWith(coordinates);
    //     InvalidTilesManager.Instance.UpdateUnavailableCoordinates();
    // }

    /// <summary>
    /// Set the type of the building that is currently being placed
    /// </summary>
    public static void SetCurrentBuildingType(Type newType) {
        if (CurrentAction == Actions.PLACE && CurrentBuildingBeingPlaced != null && CurrentBuildingBeingPlaced.CurrentBuildingState == Building.BuildingState.PICKED_UP) {
            CurrentBuildingBeingPlaced.GetComponent<BuildingSaverLoader>().LoadSelf();
        }
        GameObject LastBuildingObjectCreatedBackup = LastBuildingObjectCreated; //Backup is needed because if we destroy it now this script terminates
        LastBuildingObjectCreated = CreateNewBuildingGameObject(newType);
        UnityEngine.Object.Destroy(LastBuildingObjectCreatedBackup);
        currentBuildingType = newType;
        currentBuildingTypeChanged?.Invoke(newType);
    }

    /// <summary>
    /// Set the type of the building that is currently being placed and the variant of it
    /// </summary>
    /// <param name="newType">The type of the building, MUST be a IMultipleTypeBuilding</param>
    /// <param name="variant">The variant of the multiple type building</param>
    public static void SetCurrentBuildingType(Type newType, Enum variant) {
        // if (newType is not IMultipleTypeBuilding) throw new ArgumentException("newType must be a IMultipleTypeBuilding, else use SetCurrentBuildingType(Type newType)");
        Debug.Assert(variant != null, $"Type is null in SetCurrentBuildingType");
        GameObject LastBuildingObjectCreatedBackup = LastBuildingObjectCreated;
        LastBuildingObjectCreated = CreateNewBuildingGameObject(newType);
        //Set the variant
        // Building building = (Building)LastBuildingObjectCreated.GetComponent(newType);
        // Debug.Assert(building != null, $"building is null in SetCurrentBuildingToMultipleTypeBuilding");
        LastBuildingObjectCreated.GetComponent<MultipleTypeBuildingComponent>().SetType(variant);
        currentBuildingTypeChanged?.Invoke(newType);

        UnityEngine.Object.Destroy(LastBuildingObjectCreatedBackup);
    }

    private static GameObject CreateNewBuildingGameObject(Type newType) {
        currentBuildingType = newType;
        GameObject newGameObject = new GameObject(currentBuildingType.Name).AddComponent(newType).gameObject;
        newGameObject.transform.SetParent(CurrentTilemapTransform);
        CurrentBuildingBeingPlaced = newGameObject.GetComponent<Building>();
        return newGameObject;
    }

    public static void InitializeMap(out Building house, out Building shippingBin, out Building greenhouse) {
        IsLoadingSave = true;
        PlaceHouse(out house);
        PlaceBin(out shippingBin);
        PlaceGreenhouse(out greenhouse);
        IsLoadingSave = false;
        CreateNewBuilding();
    }

    public static void PlaceHouse(out Building house) {
        MapController mapController = GetMapController();
        Vector3Int housePos = mapController.GetHousePosition();
        GameObject houseGameObject = new("House");
        houseGameObject.transform.parent = CurrentTilemapTransform;
        houseGameObject.AddComponent<House>().PlaceBuilding(housePos);
        houseGameObject.GetComponent<House>().SetTier(1);
        houseGameObject.GetComponent<Tilemap>().color = new Color(1, 1, 1, 1);
        house = houseGameObject.GetComponent<House>();

    }

    public static void PlaceBin(out Building shippingBin) {
        MapController mapController = GetMapController();
        Vector3Int binPos = mapController.GetShippingBinPosition();
        GameObject houseGameObject = new("ShippingBin");
        houseGameObject.transform.parent = CurrentTilemapTransform;
        houseGameObject.AddComponent<ShippingBin>().PlaceBuilding(binPos);
        houseGameObject.GetComponent<Tilemap>().color = new Color(1, 1, 1, 1);
        shippingBin = houseGameObject.GetComponent<ShippingBin>();
    }

    public static void PlaceGreenhouse(out Building greenhouse) {
        MapController mapController = GetMapController();
        Vector3Int binPos = mapController.GetGreenhousePosition();
        GameObject houseGameObject = new("Greenhouse");
        houseGameObject.transform.parent = CurrentTilemapTransform;
        houseGameObject.AddComponent<Greenhouse>().PlaceBuilding(binPos);
        houseGameObject.GetComponent<Tilemap>().color = new Color(1, 1, 1, 1);
        greenhouse = houseGameObject.GetComponent<Greenhouse>();
    }


    /// <summary>
    /// Deletes all buildings except the house
    /// </summary>
    public static void DeleteAllBuildings(bool force = false) {
        var allBuilding = buildings.ToArray();
        foreach (Building building in allBuilding) {
            if (building == null) continue;
            if (building.gameObject == null) continue;
            // unavailableCoordinates.RemoveWhere(vec => building.BaseCoordinates.Contains(vec));
            building.DeleteBuilding(force);
        }
        // buildingGameObjects.RemoveWhere(gameObject => !(gameObject.GetComponent<Building>() is House)); //Remove everything except the house
        NotificationManager.Instance.SendNotification("Deleted all buildings", NotificationManager.Icons.InfoIcon);
    }

    public static void PlaceSavedBuilding(BuildingData buildingData) {
        GameObject go = new(buildingData.buildingType.Name);
        go.transform.SetParent(CurrentTilemapTransform);
        BuildingSaverLoader buildingLoader = go.AddComponent<BuildingSaverLoader>();
        buildingLoader.LoadBuilding(buildingData);
    }

    public static void FindAndDeleteBuilding(Vector3Int lowerLeftCorner) {
        Building building = buildings.FirstOrDefault(building => building.BaseCoordinates[0] == lowerLeftCorner);
        if (building == null) return;
        // buildings.Remove(building);
        // buildingGameObjects.Remove(building.gameObject);
        building.DeleteBuilding();

    }

    public static Type GetCurrentBuildingType() { return currentBuildingType; }
    // public static HashSet<Vector3Int> GetUnavailableCoordinates() { return unavailableCoordinates; }
    // public static HashSet<Vector3Int> GetPlantableCoordinates() { return plantableCoordinates; }
    public static List<Building> GetBuildings() { return buildings; }

    public static void SetCurrentAction(Actions action) {
        // if (CurrentAction == action) return;
        CurrentAction = action;
        InputHandler.Instance.SetCursorBasedOnCurrentAction(action);
        // if (action == Actions.DELETE || action == Actions.EDIT ) CurrentBuildingBeingPlaced.NoPreview();
        if (CurrentBuildingBeingPlaced == null) return;
        if (action == Actions.PLACE) CurrentBuildingBeingPlaced.DoBuildingPreview();
        else CurrentBuildingBeingPlaced.NoPreview();
        // else if ((CurrentBuildingBeingPlaced != null && CurrentBuildingBeingPlaced.CurrentBuildingState == Building.BuildingState.PICKED_UP) || CurrentBuildingBeingPlaced == null) CreateNewBuilding();//If there is a picked up building, dont create a new
    }

    public static void SetCurrentTilemapTransform(Transform newTransform) {
        CurrentTilemapTransform = newTransform;
        GetCamera().GetComponent<CameraController>().UpdateTilemapBounds();
    }

    //These 2 functions are proxys for the onClick functions of the buttons in the Editor
    public static bool Save() { return BuildingSaverLoader.SaveToFile(); }
    public static bool Load() { return BuildingSaverLoader.LoadFromFile(); }
    public static void SaveAndQuit() { if (Save()) Quit(); }//if the user saved then quit
    public static void CloseQuitConfirmPanel() { GameObject.FindGameObjectWithTag("QuitConfirm").GetComponent<MoveablePanel>().SetPanelToClosedPosition(); }
    public static void Quit() {
        GameObject quitConfirmPanel = GameObject.FindGameObjectWithTag("QuitConfirm");
        quitConfirmPanel.GetComponent<MoveablePanel>().SetPanelToOpenPosition();
    }

    public static void QuitApp() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
