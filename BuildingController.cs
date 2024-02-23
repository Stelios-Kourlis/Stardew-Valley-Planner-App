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
    private readonly List<Building> buildings = new List<Building>();
    private readonly List<UserAction> actionLog = new List<UserAction>();
    private Building currentBuilding;
    public Type currentBuildingType;
    private Actions currentAction;
    private readonly HashSet<Floor> floors = new HashSet<Floor>();
    private bool isUndoing = false;

    public HashSet<GameObject> buildingGameObjects = new HashSet<GameObject>();
    private GameObject lastBuildingObjectCreated;

    void Start(){
        currentBuildingType = typeof(Floor);
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
        isUndoing = true; //hack to prevent the action from being added to the action log
    }

    /// <summary>
    /// Deletes all buildings except the house
    /// </summary>
    public void DeleteAllBuildings() {//fix this
        foreach (Building building in buildings) {
            if (building is House) continue;
            unavailableCoordinates.RemoveWhere(vec => building.VectorInBaseCoordinates(vec));
            //if (unavailableCoordinatesAreVisible) RemoveInvalidTilesFromTilemap(building.GetBaseCoordinates());
            building.Delete();
        }
        buildings.RemoveAll(building => !(building is House)); //Remove everything except the house
    }

    public void UndoLastAction(){
        // if (actionLog.Count == 0) return;
        // UserAction lastAction = actionLog.Last();
        // actionLog.RemoveAt(actionLog.Count - 1);
        // // Debug.Log(lastAction.ToString() + actionLog.Count);
        // isUndoing = true;
        // switch (lastAction.action){
        //     case Actions.PLACE:
        //         DeleteBuilding(lastAction.position);
        //         break;
        //     case Actions.DELETE:
        //         PlaceBuilding(lastAction.building, lastAction.position);
        //         break;
        //     case Actions.EDIT:
        //         break;
        //     default:
        //         break;
        // }
    }

    public void HideTotalMaterialsNeeded(){
        GameObject.FindGameObjectWithTag("TotalMaterialsNeededPanel").GetComponent<RectTransform>().localPosition = new Vector3(0, 1000, 0);
    }

    public Building GetCurrentBuilding(){ return currentBuilding; }
    public void SetCurrentBuilding(Building building){ currentBuilding = building; }
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
