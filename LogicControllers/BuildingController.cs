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
    private static readonly HashSet<Vector3Int> unavailableCoordinates = new();
    private static readonly HashSet<Vector3Int> plantableCoordinates = new();
    public static readonly List<Building> buildings = new();
    /// <summary> actions the user has done, the first 2 elements always are Action, UID and then the building data  </summary>
    private static readonly Stack<UserAction> actionLog = new();
    /// <summary> actions the user has undone, the first 2 elements always are Action, UID and then the building data </summary>
    private static readonly Stack<UserAction> undoLog = new();
    public static Type currentBuildingType = typeof(Barn);
    public static Actions CurrentAction { get; private set; } = Actions.PLACE;
    public static bool IsLoadingSave { get; set; } = false;
    public static KeyValuePair<bool, Transform> isInsideBuilding = new(false, null);

    public static HashSet<GameObject> buildingGameObjects = new();
    public static GameObject LastBuildingObjectCreated { get; private set; }
    public static Building LastBuildingCreated => LastBuildingObjectCreated != null ? LastBuildingObjectCreated.GetComponent<Building>() : null;
    public static Building CurrentBuildingBeingPlaced { get; set; }

    public static void OnBuildingPlaced() {
        if (IsLoadingSave) return;
        LastBuildingObjectCreated = CreateNewBuildingGameObject(currentBuildingType);
    }

    /// <summary>
    /// Set the type of the building that is currently being placed
    /// </summary>
    public static void SetCurrentBuildingType(Type newType) {
        GameObject LastBuildingObjectCreatedBackup = LastBuildingObjectCreated; //Backup is needed because if we destroy it now this script terminates
        LastBuildingObjectCreated = CreateNewBuildingGameObject(newType);
        UnityEngine.Object.Destroy(LastBuildingObjectCreatedBackup);
    }

    /// <summary>
    /// Set the type of the building that is currently being placed and the variant of it
    /// </summary>
    /// <param name="newType">The type of the building, MUST be a IMultipleTypeBuilding</param>
    /// <param name="variant">The variant of the multiple type building</param>
    public static void SetCurrentBuildingType(Type newType, Enum variant) {
        Debug.Assert(variant != null, $"Type is null in SetCurrentBuildingType");
        GameObject LastBuildingObjectCreatedBackup = LastBuildingObjectCreated;
        LastBuildingObjectCreated = CreateNewBuildingGameObject(newType);
        //Set the variant
        IMultipleTypeBuilding building = LastBuildingObjectCreated.GetComponent(newType) as IMultipleTypeBuilding;
        Debug.Assert(building != null, $"building is null in SetCurrentBuildingToMultipleTypeBuilding");
        building.SetType(variant);

        UnityEngine.Object.Destroy(LastBuildingObjectCreatedBackup);
    }

    private static GameObject CreateNewBuildingGameObject(Type newType) {
        currentBuildingType = newType;
        GameObject newGameObject = new GameObject(currentBuildingType.Name).AddComponent(newType).gameObject;
        if (!isInsideBuilding.Key) newGameObject.transform.SetParent(GetGridTilemap().transform);
        else newGameObject.transform.SetParent(isInsideBuilding.Value);
        CurrentBuildingBeingPlaced = newGameObject.GetComponent<Building>();
        return newGameObject;
    }


    public static void InitializeMap(int Housetier) {

        static void PlaceHouse(int tier) {
            MapController mapController = GetMapController();
            Vector3Int housePos = mapController.GetCurrentMapType() switch {
                MapController.MapTypes.FourCorners => new Vector3Int(32, 27, 0),
                MapController.MapTypes.Beach => new Vector3Int(32, 57, 0),
                _ => new Vector3Int(32, 12, 0),
            };
            GameObject houseGameObject = new("House");
            houseGameObject.transform.parent = GetGridTilemap().transform;
            houseGameObject.AddComponent<House>().PlaceBuilding(housePos);
            houseGameObject.GetComponent<House>().SetTier(tier);
            houseGameObject.GetComponent<Tilemap>().color = new Color(1, 1, 1, 1);
        }

        static void PlaceBin() {
            MapController mapController = GetMapController();
            Vector3Int binPos = mapController.GetCurrentMapType() switch {
                // MapController.MapTypes.FourCorners => new Vector3Int(32, 27, 0),
                // MapController.MapTypes.Beach => new Vector3Int(32, 57, 0),
                _ => new Vector3Int(44, 14, 0),
            };
            GameObject houseGameObject = new("ShippingBin");
            houseGameObject.transform.parent = GetGridTilemap().transform;
            houseGameObject.AddComponent<ShippingBin>().PlaceBuilding(binPos);
            houseGameObject.GetComponent<Tilemap>().color = new Color(1, 1, 1, 1);
        }

        static void PlaceGreenhouse() {
            MapController mapController = GetMapController();
            Vector3Int binPos = mapController.GetCurrentMapType() switch {
                // MapController.MapTypes.FourCorners => new Vector3Int(32, 27, 0),
                // MapController.MapTypes.Beach => new Vector3Int(32, 57, 0),
                _ => new Vector3Int(-2, 13, 0),
            };
            GameObject houseGameObject = new("Greenhouse");
            houseGameObject.transform.parent = GetGridTilemap().transform;
            houseGameObject.AddComponent<Greenhouse>().PlaceBuilding(binPos);
            houseGameObject.GetComponent<Tilemap>().color = new Color(1, 1, 1, 1);
        }

        IsLoadingSave = true;
        PlaceHouse(Housetier);
        PlaceBin();
        PlaceGreenhouse();
        IsLoadingSave = false;
        OnBuildingPlaced();
    }

    /// <summary>
    /// Deletes all buildings except the house
    /// </summary>
    public static void DeleteAllBuildings(bool deleteHouse = false) {
        foreach (Building building in buildings) {
            if (building == null) continue;
            if (building.gameObject == null) continue;
            // if (building is House && !deleteHouse) continue;
            unavailableCoordinates.RemoveWhere(vec => building.BaseCoordinates.Contains(vec));
            building.DeleteBuilding();
        }
        // buildingGameObjects.RemoveWhere(gameObject => !(gameObject.GetComponent<Building>() is House)); //Remove everything except the house
        GetNotificationManager().SendNotification("Deleted all buildings", NotificationManager.Icons.InfoIcon);
    }

    public static void AddActionToLog(UserAction action) {
        actionLog.Push(action);
        undoLog.Clear();
    }

    public static void UndoLastAction() {
        foreach (var item in actionLog) {
            Debug.Log(item);
        }
        if (actionLog.Count == 0) return;
        UserAction lastAction = actionLog.Pop();
        Debug.Log($"Got {lastAction}");
        undoLog.Push(lastAction);
        Debug.Log($"Undoing {lastAction.action} with data {lastAction.buildingData}");
        Actions action = lastAction.action;
        switch (action) {
            case Actions.PLACE:
                DeleteBuilding(lastAction.UID);
                break;
            case Actions.DELETE:
            case Actions.EDIT:
                PlaceSavedBuilding(lastAction.buildingData);
                break;
            default:
                throw new ArgumentException($"Invalid action {action}");
        }
    }

    public static void RedoLastUndo() {
        Debug.Log("Entering Redo");
        if (undoLog.Count == 0) return;
        UserAction lastAction = undoLog.Pop();
        actionLog.Push(lastAction);
        Debug.Log($"Undoing {lastAction.action} with data {lastAction.buildingData}");
        Actions action = lastAction.action;
        switch (action) {
            case Actions.PLACE:
                PlaceSavedBuilding(lastAction.buildingData);
                break;
            case Actions.DELETE:
            case Actions.EDIT:
                DeleteBuilding(lastAction.UID);
                break;
            default:
                throw new ArgumentException($"Invalid action {action}");
        }
    }

    public static void PlaceSavedBuilding(string buildingData) {
        string[] data = buildingData.Split('|');
        Type type = Type.GetType(data[0]);
        GameObject go = new(type.Name);
        go.transform.parent = GetGridTilemap().transform;
        Building building = go.AddComponent(type) as Building;
        if (building != null) building.OnAwake();
        if (building != null) building.LoadBuildingFromData(data.Skip(1).ToArray()); //todo maybe skip first part?
    }

    public static void DeleteBuilding(int buildingUID) {
        for (int index = 0; index < GetGridTilemap().transform.childCount; index++) {
            var child = GetGridTilemap().transform.GetChild(index);
            var building = child.GetComponent<Building>();
            if (building?.UID == buildingUID) {
                building.DeleteBuilding();
                return;
            }
        };
        Debug.LogWarning($"No building with UID: {buildingUID}");
    }

    public static Type GetCurrentBuildingType() { return currentBuildingType; }
    public static HashSet<Vector3Int> GetUnavailableCoordinates() { return unavailableCoordinates; }
    public static HashSet<Vector3Int> GetPlantableCoordinates() { return plantableCoordinates; }
    public static List<Building> GetBuildings() { return buildings; }

    public static void SetCurrentAction(Actions action) {
        CurrentAction = action;
        InputHandler.CursorType type = action switch {
            Actions.PLACE => InputHandler.CursorType.Place,
            Actions.DELETE => InputHandler.CursorType.Delete,
            Actions.EDIT => InputHandler.CursorType.Pickup,
            _ => InputHandler.CursorType.Default
        };
        GetInputHandler().SetCursor(type);
    }

    //These 2 functions are proxys for the onClick functions of the buttons in the Editor
    public static bool Save() { return Utility.BuildingManager.Save(); }
    public static bool Load() { return Utility.BuildingManager.Load(); }
    public static void SaveAndQuit() { if (Save()) Quit(); }//if the user saved then quit
    public static void CloseQuitConfirmPanel() { GameObject.FindGameObjectWithTag("QuitConfirm").GetComponent<RectTransform>().localPosition = new Vector3(0, 1000, 0); }
    public static void Quit() {
        GameObject quitConfirmPanel = GameObject.FindGameObjectWithTag("QuitConfirm");
        quitConfirmPanel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
    }

    public static void QuitApp() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
    }
}
