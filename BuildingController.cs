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
    private bool buildingIsPickedUp = false;
    private Building currentBuilding;
    public Type currentBuildingType;
    private Actions currentAction;
    private readonly HashSet<Floor> floors = new HashSet<Floor>();
    private bool isUndoing = false;
    // private Dictionary<Materials, int> totalMaterialsNeeded = new Dictionary<Materials, int>();
    //private FloorType currentFloorType;

    public HashSet<GameObject> buildingGameObjects = new HashSet<GameObject>();
    private GameObject lastBuildingObjectCreated;

    void Start(){
        //currentBuilding = new SprinklerT3();
        currentBuildingType = typeof(GoldClock);
        OnBuildingPlaced();
        Building.buildingWasPlaced += OnBuildingPlaced;
        //currentFloorType = FloorType.WOOD_FLOOR;

        // Building house = new BarnT1();
        // var x = house.spriteCoordinates;
        //house.spriteCoordinates[0] = new Vector3Int(0, 0, 0);
    
    }

    void Update(){
        foreach (Building building in buildings) if (building.buildingInteractions.Length != 0) GetButtonController().UpdateButtonPositionsAndScaleForBuilding(building);
    }

    private void OnBuildingPlaced(){
        Debug.Log("PLACED");
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

    /// <summary>
    /// Creates a tilemap and places building on it
    /// </summary>
    /// <param name="building">The building to place</param>
    /// <param name="position">The lower left position of the building</param>
    public void PlaceBuilding(Type BuildingType, Vector3Int position){
        // if (building is Floor) PlaceCurrentlySelectedFloor(position);
        // if (building is Sprinkler sprinkler) PlaceSprinkler( sprinkler, position);
        //else PlaceFarmBuilding(currentBuildingType, position);
        PlaceFarmBuilding(BuildingType, position);
        
    }

    private void HandleSpecialBuilding(Building building){
        if (building is FishPond) HandleFishPond( (FishPond) building);
        if (building is Greenhouse) HandleGreenHouse( (Greenhouse) building);
    }

    private void HandleFishPond(FishPond fishPond){//todo fix this
        // GameObject parentTilemapObject = fishPond.tilemap.gameObject;
        // //Tile[] fishPondBottom = SplitSprite(new FishPondBottom(null, null, null));
        // GameObject fishPondBottomTilemapObject = CreateTilemapObject(parentTilemapObject.transform, parentTilemapObject.GetComponent<TilemapRenderer>().sortingOrder - 1, "FishPondBottom");
        // fishPondBottomTilemapObject.GetComponent<Tilemap>().SetTiles(fishPond.spriteCoordinates, fishPondBottom);
        // GameObject decoTilemapObject = CreateTilemapObject(parentTilemapObject.transform, parentTilemapObject.GetComponent<TilemapRenderer>().sortingOrder + 1, "FishPondDeco");
        // FishDeco deco = new FishDeco(GetAreaAroundPosition(fishPond.spriteCoordinates[0], 3, 5).ToArray<Vector3Int>(), decoTilemapObject.GetComponent<Tilemap>());
        // Tile[] decoTiles = deco.GetDeco(0);
        
        // decoTilemapObject.GetComponent<Tilemap>().SetTiles(deco.GetPosition(), decoTiles);
        // fishPond.deco = deco;
        //fishPond.SetFishImage();
    }

    private void HandleGreenHouse(Building greenhouse){
        GameObject parentTilemapObject = greenhouse.tilemap.gameObject;
        Debug.Log("Placeing Greenhouse");
        List<Vector3Int> extraFrontArea = GetAreaAroundPosition(new Vector3Int(greenhouse.baseCoordinates[0].x + 2, greenhouse.baseCoordinates[0].y - 2, 0), 2, 3);
        unavailableCoordinates.UnionWith(extraFrontArea);
        Sprite greenhouseFrontTileSprite = Sprite.Create(Resources.Load("GreenhouseFront") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);//todo Tile is wrong painted (pixelart issue)
        Tile greenhouseFrontTile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        greenhouseFrontTile.sprite = greenhouseFrontTileSprite;
        foreach (Vector3Int cell in extraFrontArea){
            parentTilemapObject.gameObject.GetComponent<Tilemap>().SetTile(cell, greenhouseFrontTile);
        }
        
    }

    // private void PlaceFarmBuilding(Building building, Vector3Int position){
    //     List<Vector3Int> baseCoordinates = GetAreaAroundPosition(position, building.baseHeight, building.width);
    //     if (unavailableCoordinates.Intersect(baseCoordinates).Count() != 0) return;
    //     List<Vector3Int> spriteCoordinates = GetAreaAroundPosition(position, building.height, building.width, true);
    //     Tile[] buildingTiles = SplitSprite(building);
    //     GameObject tilemapObject = CreateTilemapObject(transform, -position.y + 50, building.name);
    //     tilemapObject.GetComponent<Tilemap>().SetTiles(spriteCoordinates.ToArray<Vector3Int>(), buildingTiles);
    //     Building newBuilding = Activator.CreateInstance(building.GetType(), spriteCoordinates.ToArray(), baseCoordinates.ToArray(), tilemapObject.GetComponent<Tilemap>()) as Building;
    //     buildings.Add(newBuilding);
    //     if (!isUndoing) actionLog.Add(new UserAction(Actions.PLACE, building, position));
    //     isUndoing = false;
    //     unavailableCoordinates.UnionWith(baseCoordinates);
        
    //     GetButtonController().CreateButtonsForBuilding(newBuilding);
    //     if (buildingIsPickedUp == true) currentAction = Actions.EDIT;
    //     GetMapController().UpdateUnavailableCoordinates();
    //     if (IsSpecialBuilding(building)) HandleSpecialBuilding(newBuilding);
    // }

    private void PlaceFarmBuilding(Type building, Vector3Int position){
        //GameObject buildingObject = new GameObject(building.Name);
        // buildingObject.transform.parent = transform;
        // Building build = (Building) buildingObject.AddComponent(building);
        // build.PlaceBuilding(position);
    }

    private void PlaceSprinkler(Sprinkler sprinkler, Vector3Int position){
        if (unavailableCoordinates.Contains(position)) return;
        Tile buildingTile = SplitSprite(sprinkler)[0];
        GameObject tilemapObject = CreateTilemapObject(transform, -position.y + 50, sprinkler.name);
        tilemapObject.GetComponent<Tilemap>().SetTile(position, buildingTile);
        Sprinkler newSprinkler = Activator.CreateInstance(sprinkler.GetType(), new Vector3Int[]{position}, new Vector3Int[]{position}, tilemapObject.GetComponent<Tilemap>()) as Sprinkler;
        if (!isUndoing) actionLog.Add(new UserAction(Actions.PLACE, sprinkler, position));
        isUndoing = false;
        unavailableCoordinates.Add(position);
        if (buildingIsPickedUp == true) currentAction = Actions.EDIT;
        GetMapController().UpdateUnavailableCoordinates();
    }

    /// <summary>
    /// Creates a tilemap and places BuildingPlacer's currentBuilding on it
    /// </summary>
    /// <param name="position">The lower left position of the building</param>
    public void PlaceCurrentlySelectedBuilding(Vector3Int position){
        PlaceBuilding(currentBuildingType, position);
    }

    /// <summary>
    /// Delete the building at the position given
    /// </summary>
    /// <param name=""> The position the building you want to delete is on, can be any of the base coordinates not just bottom left</param>
    public void DeleteBuilding(Vector3Int position){
        //if (floors.Any(floor => floor.GetPosition() == position)) 
        if (!unavailableCoordinates.Contains(position)){
            DeleteFloor(position);
            return;
        }
        Building building = buildings.FirstOrDefault(building => building.VectorInBaseCoordinates(position) && !(building is House));
        if (building == null) return;
        buildings.Remove(building);
        unavailableCoordinates.RemoveWhere(building.VectorInBaseCoordinates);
        building.Delete();
        if (!isUndoing) actionLog.Add(new UserAction(Actions.DELETE, building, building.baseCoordinates[0]));
        isUndoing = false;
        GetMapController().UpdateUnavailableCoordinates();
    }
    
    public void PickupBuilding(Vector3Int position){//todo ALL NBT DATA IS LOST ON MOVE
        if (!unavailableCoordinates.Contains(position)) return;
        Building building = buildings.FirstOrDefault(building => building.VectorInBaseCoordinates(position) && !(building is House));
        if (building == null) return;
        buildings.Remove(building);
        unavailableCoordinates.RemoveWhere(building.VectorInBaseCoordinates);
        building.Delete();
        // currentBuilding = Activator.CreateInstance(building.GetType(), null, null, null) as Building;
        currentBuilding = building;
        currentAction = Actions.PLACE;
        buildingIsPickedUp = true;
        return;
    }
         
    public void PlaceCurrentlySelectedFloor(Vector3Int position) {//todo fix
        // if (unavailableCoordinates.Contains(position)) return;
        // if (!(currentBuilding is Floor)) return;
        // floors.RemoveWhere(floor => floor.GetPosition() == position);
        // FloorType type = ((Floor) currentBuilding).GetFloorType();
        // Floor floor = new Floor(position, type);
        // HashSet<FloorFlag> flags = GetFloorFlags(floor, floors);
        // Tile floorTile = floor.GetFloorConfig(flags.ToArray(), type);
        // Tilemap floorTilemap = gameObject.transform.Find("FloorTilemap").GetComponent<Tilemap>();
        // floorTilemap.SetTile(position, floorTile);
        // floors.Add(floor);
        // if (!isUndoing) actionLog.Add(new UserAction(Actions.PLACE, floor, position));
        // isUndoing = false;
        // foreach (Floor neighborFloor in floors){
        //     if (GetNeighboursOfPosition(position).Contains(neighborFloor.GetPosition())) UpdateFloor(neighborFloor.GetPosition());
        // }
    }

    public void UpdateFloor(Vector3Int position){//todo fix
        // FloorType type = floors.FirstOrDefault(floor => floor.GetPosition() == position).GetFloorType();
        // Floor floor = new Floor(position, type);
        // HashSet<FloorFlag> flags = GetFloorFlags(floor, floors);
        // Tile floorTile = floor.GetFloorConfig(flags.ToArray(), type);
        // Tilemap floorTilemap = gameObject.transform.Find("FloorTilemap").GetComponent<Tilemap>();
        // floorTilemap.SetTile(position, floorTile);
    }

    public void DeleteFloor(Vector3Int position){
        Floor floor = floors.FirstOrDefault(floor => floor.GetPosition() == position);
        if (floor == null) return;
        floors.Remove(floor);
        Tilemap floorTilemap = gameObject.transform.Find("FloorTilemap").GetComponent<Tilemap>();
        floorTilemap.SetTile(position, null);
        if (!isUndoing) actionLog.Add(new UserAction(Actions.DELETE, floor, position));
        isUndoing = false;
        foreach (Floor neighborFloor in floors){
            if (GetNeighboursOfPosition(position).Contains(neighborFloor.GetPosition())) UpdateFloor(neighborFloor.GetPosition());
        }
    }

    public void PlaceHouse(int tier) {
        MapController mapController = GetMapController();
        Vector3Int housePos = mapController.GetCurrentMapType() switch{
            MapTypes.FourCorners => new Vector3Int(32, 27, 0),
            MapTypes.Beach => new Vector3Int(33, 57, 0),
            _ => new Vector3Int(32, 12, 0),
        };

        Building house = buildings.FirstOrDefault<Building>(building => building is House);
        if (house != null) {
            foreach (Vector3Int cell in house.baseCoordinates) unavailableCoordinates.Remove(cell);
            buildings.Remove(house);
            house.Delete();
        }
        house = tier switch{
            1 => new HouseT1(),
            2 => new HouseT2(),
            3 => new HouseT3(),
            _ => null,
        };
        GameObject.Destroy(house.buttonParent);
        isUndoing = true; //hack to prevent the action from being added to the action log
        //PlaceBuilding(house, housePos);//todo fix this
    }

    /// <summary>
    /// Deletes all buildings except the house
    /// </summary>
    public void DeleteAllBuildings() {
        foreach (Building building in buildings) {
            if (building is House) continue;
            unavailableCoordinates.RemoveWhere(vec => building.VectorInBaseCoordinates(vec));
            //if (unavailableCoordinatesAreVisible) RemoveInvalidTilesFromTilemap(building.GetBaseCoordinates());
            building.Delete();
        }
        buildings.RemoveAll(building => !(building is House)); //Remove everything except the house
    }

    //todo this needs to go elsewhere
    public void CycleFishPondDeco(Building building){
        if (building == null || !(building is FishPond)) return;
        FishPond fishPond = (FishPond) building;
        Tile[] decoTiles = fishPond.deco.GetNextDeco();
        Tilemap decoTilemap = fishPond.deco.tilemap;
        decoTilemap.SetTiles(fishPond.deco?.GetPosition(), decoTiles);
        //building.GetTilemap().gameObject.transform.GetChild(2).
    }

    public void ToggleBuildingButtons(Vector3Int position){
        Building building = buildings.FirstOrDefault(building => building.VectorInBaseCoordinates(position));
        if (building == null) return;
        bool buttonsAreActive = building.buttonParent.activeInHierarchy;
        building.buttonParent.SetActive(!buttonsAreActive);
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
