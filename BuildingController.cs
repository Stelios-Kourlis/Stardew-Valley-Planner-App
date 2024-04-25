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

public class BuildingController : MonoBehaviour{
    /// <summary> A coordinate is unavailable if it is occupied by a building or if its out of bounds for the current map </summary>
    private readonly HashSet<Vector3Int> unavailableCoordinates = new();
    private readonly HashSet<Vector3Int> plantableCoordinates = new();
    public readonly List<Building> buildings = new();
    /// <summary> actions the user has done, the first 2 elements always are Action, UID and then the building data  </summary>
    private readonly Stack<UserAction> actionLog = new();
    /// <summary> actions the user has undone, the first 2 elements always are Action, UID and then the building data </summary>
    private readonly Stack<UserAction> undoLog = new();
    public Type currentBuildingType;
    // private Actions currentAction;
    // private readonly HashSet<Floor> floors = new HashSet<Floor>();
    public bool IsLoadingSave {get; set;} = false;
    //private bool isUndoing = false;

    public HashSet<GameObject> buildingGameObjects = new();
    public GameObject lastBuildingObjectCreated {get; private set;}

    void Start(){
        currentBuildingType = typeof(Barn);
        Building.BuildingWasPlaced += OnBuildingPlaced;
    }

    void Update(){
        if (Building.CurrentAction == Actions.EDIT){
            if (lastBuildingObjectCreated != null) Destroy(lastBuildingObjectCreated);
        }
        else if (Building.CurrentAction == Actions.PLACE){
            if (lastBuildingObjectCreated == null) OnBuildingPlaced();
        }
    }

    private void OnBuildingPlaced(){
        if (IsLoadingSave) return;
        // Debug.Log("Building Placed");
        GameObject go = new(currentBuildingType.Name);
        go.transform.parent = transform;
        go.AddComponent(currentBuildingType);
        lastBuildingObjectCreated = go;
    }

    public void SetCurrentBuildingType(Type newType){
        lastBuildingObjectCreated.GetComponent<Building>().ForceDelete();
        currentBuildingType = newType;
        lastBuildingObjectCreated.AddComponent(currentBuildingType);
        lastBuildingObjectCreated.name = currentBuildingType.Name;
    }

    public void SetCurrentBuildingToMultipleTypeBuilding<T>(Type buildingType, T type) where T : Enum{
        Debug.Assert(type != null, $"Type is null in SetCurrentBuildingToMultipleTypeBuilding");
        lastBuildingObjectCreated.GetComponent<Building>().ForceDelete();
        currentBuildingType = buildingType;
        IMultipleTypeBuilding<T> building = lastBuildingObjectCreated.AddComponent(buildingType) as IMultipleTypeBuilding<T>;
        Debug.Assert(building != null, $"building is null in SetCurrentBuildingToMultipleTypeBuilding");
        building.SetType(type);
        lastBuildingObjectCreated.name = currentBuildingType.Name;
    }

    public void PlaceHouse(int tier) {
        MapController mapController = GetMapController();
        Vector3Int housePos = mapController.GetCurrentMapType() switch{
            MapController.MapTypes.FourCorners => new Vector3Int(32, 27, 0),
            MapController.MapTypes.Beach => new Vector3Int(33, 57, 0),
            _ => new Vector3Int(32, 12, 0),
        };
        GameObject houseGameObject = new("House");
        houseGameObject.transform.parent = transform;
        houseGameObject.AddComponent<House>().OnAwake();
        houseGameObject.GetComponent<House>().Place(housePos);
        houseGameObject.GetComponent<House>().SetTier(tier);
        houseGameObject.GetComponent<Tilemap>().color = new Color(1,1,1,1);
        // isUndoing = true; //hack to prevent the action from being added to the action log
    }

    /// <summary>
    /// Deletes all buildings except the house
    /// </summary>
    public void DeleteAllBuildings(bool deleteHouse = false) {
        foreach (Building building in buildings) {
            // if (building is House && !deleteHouse) continue;
            unavailableCoordinates.RemoveWhere(vec => building.VectorInBaseCoordinates(vec));
            building.ForceDelete();
        }
        // buildingGameObjects.RemoveWhere(gameObject => !(gameObject.GetComponent<Building>() is House)); //Remove everything except the house
        GetNotificationManager().SendNotification("Deleted all buildings");
    }

    public void AddActionToLog(UserAction action){
        actionLog.Push(action);
        undoLog.Clear();
    }

    public void UndoLastAction(){
        foreach(var item in actionLog){
            Debug.Log(item);
        }
        if (actionLog.Count == 0) return;
        UserAction lastAction = actionLog.Pop();
        Debug.Log($"Got {lastAction}");
        undoLog.Push(lastAction);
        Debug.Log($"Undoing {lastAction.action} with data {lastAction.buildingData}");
        Actions action = lastAction.action;
        switch (action){
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

    public void RedoLastUndo(){
        Debug.Log("Entering Redo");
        if (undoLog.Count == 0) return;
        UserAction lastAction = undoLog.Pop();
        actionLog.Push(lastAction);
        Debug.Log($"Undoing {lastAction.action} with data {lastAction.buildingData}");
        Actions action = lastAction.action;
        switch (action){
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

    public void PlaceSavedBuilding(string buildingData){
        string[] data = buildingData.Split('|');
        Type type = Type.GetType(data[0]);
        int x = int.Parse(data[1]);
        int y = int.Parse(data[2]);
        GameObject go = new(type.Name);
        go.transform.parent = transform;
        Building building = go.AddComponent(type) as Building;
        if (building != null) building.OnAwake();
        if (building != null) building.RecreateBuildingForData(x, y, data.Skip(3).ToArray());
    }

    public void DeleteBuilding(int buildingUID){
        for (int index = 0; index < transform.childCount; index++){
            var child = transform.GetChild(index);
            var building = child.GetComponent<Building>();
            if (building?.UID == buildingUID){
                building.Delete();
                return;
            }
        };
        Debug.LogWarning($"No building with UID: {buildingUID}");
    }

    public Type GetCurrentBuildingType(){ return currentBuildingType; }
    public HashSet<Vector3Int> GetUnavailableCoordinates(){ return unavailableCoordinates; }
    public HashSet<Vector3Int> GetPlantableCoordinates(){ return plantableCoordinates; }
    public List<Building> GetBuildings(){ return buildings; }
    // public Actions GetCurrentAction(){ return currentAction; }
    //public FloorType GetCurrentFloorType(){ return currentFloorType; }
    // public HashSet<Floor> GetFloors(){ return floors; }
    [Obsolete("Use Building.CurrentAction instead")]
    public void SetCurrentAction(Actions action){ Building.CurrentAction = action; }

    //These 2 functions are proxys for the onClick functions of the buttons in the Editor
    public bool Save(){ return Utility.BuildingManager.Save(); }
    public bool Load(){ return Utility.BuildingManager.Load(); }
    public void SaveAndQuit(){ if (Save()) Quit(); }//if the user saved then quit
    public void CloseQuitConfirmPanel() { GameObject.FindGameObjectWithTag("QuitConfirm").GetComponent<RectTransform>().localPosition = new Vector3(0, 1000, 0); }
    public void Quit() {
        GameObject quitConfirmPanel = GameObject.FindGameObjectWithTag("QuitConfirm");
        quitConfirmPanel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
    }

    public void QuitApp(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
