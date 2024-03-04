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
    private readonly HashSet<Vector3Int> unavailableCoordinates = new HashSet<Vector3Int>();
    public readonly List<Building> buildings = new List<Building>();
    /// <summary> actions the user has done, the first 2 elements always are Action, UID and then the building data  </summary>
    private readonly Stack<UserAction> actionLog = new Stack<UserAction>();
    /// <summary> actions the user has undone, the first 2 elements always are Action, UID and then the building data </summary>
    private readonly Stack<UserAction> undoLog = new Stack<UserAction>();
    public Type currentBuildingType;
    // private Actions currentAction;
    // private readonly HashSet<Floor> floors = new HashSet<Floor>();
    public bool IsLoadingSave {get; set;} = false;
    //private bool isUndoing = false;

    public HashSet<GameObject> buildingGameObjects = new HashSet<GameObject>();
    private GameObject lastBuildingObjectCreated;

    void Start(){
        currentBuildingType = typeof(Barn);
        Building.buildingWasPlaced += OnBuildingPlaced;
    
    }

    void Update(){
        if (Building.currentAction == Actions.EDIT){
            if (lastBuildingObjectCreated != null) Destroy(lastBuildingObjectCreated);
        }
        else if (Building.currentAction == Actions.PLACE){
            if (lastBuildingObjectCreated == null) OnBuildingPlaced();
        }

        //foreach (Building building in buildings) if (building.buildingInteractions.Length != 0) GetButtonController().UpdateButtonPositionsAndScaleForBuilding(building);
    }

    private void OnBuildingPlaced(){
        if (IsLoadingSave) return;
        GameObject go = new GameObject(currentBuildingType.Name);
        go.transform.parent = transform;
        lastBuildingObjectCreated = go;
        go.AddComponent(currentBuildingType);
    }

    public void SetCurrentBuildingType(Type newType){
        Component component = lastBuildingObjectCreated.GetComponent(currentBuildingType);
        if (component != null) Destroy(component);
        currentBuildingType = newType;
        lastBuildingObjectCreated.AddComponent(currentBuildingType);
    }

    public void PlaceHouse(int tier) {
        MapController mapController = GetMapController();
        Vector3Int housePos = mapController.GetCurrentMapType() switch{
            MapTypes.FourCorners => new Vector3Int(32, 27, 0),
            MapTypes.Beach => new Vector3Int(33, 57, 0),
            _ => new Vector3Int(32, 12, 0),
        };
        GameObject houseGameObject = new GameObject("House");
        houseGameObject.transform.parent = transform;
        houseGameObject.AddComponent<House>().Start();
        houseGameObject.GetComponent<House>().Place(housePos);
        houseGameObject.GetComponent<House>().ChangeTier(tier);
        houseGameObject.GetComponent<Tilemap>().color = new Color(1,1,1,1);
        //isUndoing = true; //hack to prevent the action from being added to the action log
    }

    /// <summary>
    /// Deletes all buildings except the house
    /// </summary>
    public void DeleteAllBuildings(bool deleteHouse = false) {
        foreach (Building building in buildings) {
            if (building is House && !deleteHouse) continue;
            unavailableCoordinates.RemoveWhere(vec => building.VectorInBaseCoordinates(vec));
            building.ForceDelete();
        }
        buildingGameObjects.RemoveWhere(gameObject => !(gameObject.GetComponent<Building>() is House)); //Remove everything except the house
    }

    public void AddActionToLog(UserAction action){
        actionLog.Push(action);
        undoLog.Clear();
    }

    public void UndoLastAction(){
        Debug.Log("Entering Undo");
        if (actionLog.Count == 0) return;
        UserAction lastAction = actionLog.Pop();
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

    public void HideTotalMaterialsNeeded(){
        GameObject.FindGameObjectWithTag("TotalMaterialsNeededPanel").GetComponent<RectTransform>().localPosition = new Vector3(0, 1000, 0);
    }

    public void PlaceSavedBuilding(string buildingData){
        string[] data = buildingData.Split('|');
        Type type = Type.GetType(data[0]);
        int x = int.Parse(data[1]);
        int y = int.Parse(data[2]);
        GameObject go = new GameObject(type.Name);
        go.transform.parent = transform;
        Building building = go.AddComponent(type) as Building;
        building?.Start();
        building?.RecreateBuildingForData(x, y, data.Skip(3).ToArray());
    }

    public void DeleteBuilding(int buildingUID){
        for (int index = 0; index < transform.childCount; index++){
            var child = transform.GetChild(index);
            var building = child.GetComponent<Building>();
            if (building?.UID == buildingUID){
                building.ForceDelete();
                return;
            }
        };
        Debug.LogWarning($"No building with UID: {buildingUID}");
    }

    public Type GetCurrentBuildingType(){ return currentBuildingType; }
    public HashSet<Vector3Int> GetUnavailableCoordinates(){ return unavailableCoordinates; }
    public List<Building> GetBuildings(){ return buildings; }
    // public Actions GetCurrentAction(){ return currentAction; }
    //public FloorType GetCurrentFloorType(){ return currentFloorType; }
    // public HashSet<Floor> GetFloors(){ return floors; }
    public void SetCurrentAction(Actions action){ Building.currentAction = action; }

    //These 2 functions are proxys for the onClick functions of the buttons in the Editor
    public void Save(){ Utility.BuildingManager.Save(); }
    public void Load(){ Utility.BuildingManager.Load(); }
}
