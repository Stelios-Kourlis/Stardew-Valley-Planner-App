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

public class BuildingController : MonoBehaviour
{
    /// <summary> A coordinate is unavailable if it is occupied by a building or if its out of bounds for the current map </summary>
    private readonly HashSet<Vector3Int> unavailableCoordinates = new HashSet<Vector3Int>();
    public readonly List<Building> buildings = new List<Building>();
    private readonly List<string> actionLog = new List<string>();
    private readonly List<string> undoLog = new List<string>();
    public Type currentBuildingType;
    private Actions currentAction;
    private readonly HashSet<Floor> floors = new HashSet<Floor>();
    public bool isLoadingSave {get; set;} = false;
    //private bool isUndoing = false;

    public HashSet<GameObject> buildingGameObjects = new HashSet<GameObject>();
    private GameObject lastBuildingObjectCreated;

    void Start(){
        currentBuildingType = typeof(GoldClock);
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
        if (isLoadingSave) return;
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

    public void AddActionToLog(string action){
        actionLog.Add(action);
        Debug.Log("--||--");
        undoLog.Clear();
        foreach (string act in actionLog) Debug.Log(act);
    }

    public void UndoLastAction(){
        if (actionLog.Count == 0) return;
        string lastAction = actionLog.Last();
        undoLog.Add(lastAction);
        Debug.Log($"Undoing {lastAction}");
        actionLog.RemoveAt(actionLog.Count - 1);
        string[] data = lastAction.Split('|');
        Actions action = (Actions) Enum.Parse(typeof(Actions), data[0]);
        data = data.Skip(1).ToArray();
        switch (action){
            case Actions.PLACE:
                PlaceSavedBuilding(string.Join("|", data));
                break;
            case Actions.DELETE:
                DeleteBuilding(int.Parse(data[0]));
                break;
            case Actions.EDIT:
                break;
            default:
                throw new System.ArgumentException($"Invalid action {action}");
        }
    }

    public void HideTotalMaterialsNeeded(){
        GameObject.FindGameObjectWithTag("TotalMaterialsNeededPanel").GetComponent<RectTransform>().localPosition = new Vector3(0, 1000, 0);
    }

    public void PlaceSavedBuilding(string buildingData){
        //DeleteAllBuildings(true);
        string[] data = buildingData.Split('|');
        Type type = Type.GetType(data[0]);
        int x = int.Parse(data[1]);
        int y = int.Parse(data[2]);
        GameObject go = new GameObject(type.Name);
        go.transform.parent = transform;
        Building building = go.AddComponent(type) as Building;
        building?.Start();
        // Debug.Log(type);
        building?.RecreateBuildingForData(x, y, data.Skip(3).ToArray());

        //PlaceBuilding(DeepCopyOfBuilding(name), new Vector3Int(x, y, 0));
    }

    public void DeleteBuilding(int buildingUID){
        for (int index = 0; index < transform.childCount; index++){
            var child = transform.GetChild(index);
            var building = child.GetComponent<Building>();
            Debug.Log($"Checking {building?.UID} from {child.name} against {buildingUID}");
            if (building?.UID == buildingUID){
                Debug.Log("Found a match");
                building.ForceDelete();
                return;
            }
        };
    }

    public Type GetCurrentBuildingType(){ return currentBuildingType; }
    public HashSet<Vector3Int> GetUnavailableCoordinates(){ return unavailableCoordinates; }
    public List<Building> GetBuildings(){ return buildings; }
    public Actions GetCurrentAction(){ return currentAction; }
    //public FloorType GetCurrentFloorType(){ return currentFloorType; }
    public HashSet<Floor> GetFloors(){ return floors; }
    public void SetCurrentAction(Actions action){ Building.currentAction = action; }

    //These 2 functions are proxys for the onClick functions of the buttons in the Editor
    public void Save(){ Utility.BuildingManager.Save(); }
    public void Load(){ Utility.BuildingManager.Load(); }
}
