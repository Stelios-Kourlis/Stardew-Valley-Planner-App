using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using System;

public class InputHandler : MonoBehaviour {

    Tile redTile, greenTile;
    BuildingController buildingController;
    bool mouseIsHeld = false;
    Vector3Int mousePositionWhenHoldStarted;
    Tilemap buildingPreviewTilemap, buildingBasePreviewTilemap;

    void Start() {
        Sprite redTileSprite = Sprite.Create(Resources.Load("RedTile") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        redTile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        redTile.sprite = redTileSprite;

        Sprite greenTileSprite = Sprite.Create(Resources.Load("GreenTile") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        greenTile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        greenTile.sprite = greenTileSprite;

        GameObject[] tilemaps = GameObject.FindGameObjectsWithTag("MouseTileMap");
        buildingPreviewTilemap = tilemaps[0].GetComponent<Tilemap>(); //the translucent version of the building
        buildingBasePreviewTilemap = tilemaps[1].GetComponent<Tilemap>(); //the red/green tiles representing the structure's base and follow the mouse

        buildingController = GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingController>();

        Cursor.SetCursor(Resources.Load("UI/Cursor") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
    }

    void Update(){
        //Debug.Log(buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        
        //HandleMouseMove();

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            mouseIsHeld = true; 
            mousePositionWhenHoldStarted = buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)){
            mouseIsHeld = false;
            HandleMouseLeftClick(); 
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z) ){
            buildingController.UndoLastAction();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) buildingController.ToggleBuildingButtons(buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));//todo

        if (Input.GetKeyUp(KeyCode.I)) GetMapController().ToggleMapUnavailableCoordinates();

        if (Input.GetKeyUp(KeyCode.Escape)) GameObject.FindGameObjectWithTag("SettingsButton").GetComponent<SettingsButton>().ToggleSettingsModal();

        if (Input.GetKeyUp(KeyCode.S)) buildingController.Save();

        if (Input.GetKeyUp(KeyCode.L)) buildingController.Load();

        if (Input.GetKeyUp(KeyCode.D)) GameObject.FindGameObjectWithTag("DeleteAllButton").GetComponent<ConfirmationWidow>().OpenConfirmDialog();
    }
    /// <summary> Creates the red/green tiles representing the structure's base and follow the mouse as well as the translucent version of the current building </summary>
    public void PlaceMouseoverEffect() {
        Type buildingType = buildingController.currentBuildingType;
        Vector3Int currentCell = buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        HashSet<Vector3Int> unavailableCoordinates = buildingController.GetUnavailableCoordinates();

        buildingPreviewTilemap.ClearAllTiles();
        buildingBasePreviewTilemap.ClearAllTiles();

        //buildingPreviewTilemap.SetTiles(GetAreaAroundPosition(currentCell, building.height, building.width).ToArray(), SplitSprite(building, false));
        //--
        // Building building = buildingController.GetCurrentBuilding();
        // if (building is Floor){
        //     PlaceFloorMouseoverEffect();
        //     return;
        // }
        // if (building is Sprinkler){
        //     PlaceSprinklerMouseoverEffect();
        //     return;
        // }
        // Vector3Int currentCell = buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        // HashSet<Vector3Int> unavailableCoordinates = buildingController.GetUnavailableCoordinates();

        // Vector3Int[] mouseoverEffectArea;

        // mouseTilemap1.GetComponent<TilemapRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        // mouseTilemap1.ClearAllTiles();
        // mouseoverEffectArea = mouseoverEffectArea = GetAreaAroundPosition(currentCell, building.height, building.width).ToArray();;

        // mouseTilemap1.SetTiles(mouseoverEffectArea, SplitSprite(building, false));

        // buildingPreviewTilemap.ClearAllTiles();
        // mouseoverEffectArea = GetAreaAroundPosition(currentCell, building.baseHeight, building.width).ToArray();
        // foreach (Vector3Int vector in mouseoverEffectArea) {
        //     if (unavailableCoordinates.Contains(vector)) buildingPreviewTilemap.SetTile(vector, redTile);
        //     else buildingPreviewTilemap.SetTile(vector, greenTile);
        // }
    }

    // public void PlaceSprinklerMouseoverEffect(){
    //     Building currentBuilding = buildingController.GetCurrentBuilding();

    //     Vector3Int currentCell = buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    //     HashSet<Vector3Int> unavailableCoordinates = buildingController.GetUnavailableCoordinates();
        
        

    //     buildingPreviewTilemap.GetComponent<TilemapRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    //     buildingPreviewTilemap.ClearAllTiles();
    //     buildingPreviewTilemap.SetTile(currentCell, SplitSprite(currentBuilding, false)[0]);

    //     buildingBasePreviewTilemap.ClearAllTiles();
    //     Vector3Int[] mouseoverEffectArea = null;
    //     if (currentBuilding is SprinklerT1) mouseoverEffectArea = GetCrossAroundPosition(currentCell).ToArray();
    //     if (currentBuilding is SprinklerT2) mouseoverEffectArea = GetAreaAroundPosition(currentCell, 1).ToArray();
    //     if (currentBuilding is SprinklerT3) mouseoverEffectArea = GetAreaAroundPosition(currentCell, 2).ToArray();
    //     foreach (Vector3Int vector in mouseoverEffectArea) {
    //         if (unavailableCoordinates.Contains(vector)) buildingBasePreviewTilemap.SetTile(vector, redTile);
    //         else buildingBasePreviewTilemap.SetTile(vector, greenTile);
    //     }


    // }
        

    public void EditMouseoverEffect(){
        Vector3Int currentCell = buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        buildingPreviewTilemap.GetComponent<TilemapRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        buildingPreviewTilemap.ClearAllTiles();
        buildingBasePreviewTilemap.ClearAllTiles();

        buildingPreviewTilemap.SetTile(currentCell, greenTile);
    }

    public void PlaceFloorMouseoverEffect() {
        // Vector3Int currentCell = buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        // HashSet<Vector3Int> unavailableCoordinates = buildingController.GetUnavailableCoordinates();

        // buildingPreviewTilemap.GetComponent<TilemapRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        // buildingPreviewTilemap.ClearAllTiles();
        // buildingBasePreviewTilemap.ClearAllTiles();

        // Building currentBuilding = buildingController.GetCurrentBuilding();
        // if (!(currentBuilding is Floor)) return;
        // HashSet<Vector3Int> placeFloorArea;
        // if (mouseIsHeld)  placeFloorArea = GetAllCoordinatesInArea(currentCell, mousePositionWhenHoldStarted);
        // else placeFloorArea = new HashSet<Vector3Int>(){currentCell};
        // foreach (Vector3Int cell in placeFloorArea){
        //     //Floor floor = new Floor(cell, ((Floor) currentBuilding).GetFloorType());
        //     Tile floorTile = floor.GetFloorConfig(new HashSet<FloorFlag>().ToArray(), floor.GetFloorType());//todo change floor type, maybe correctly show floor flags
        //     buildingPreviewTilemap.SetTile(cell, floorTile);
        //     if (unavailableCoordinates.Contains(floor.GetPosition())) buildingBasePreviewTilemap.SetTile(floor.GetPosition(), redTile);
        //     else buildingBasePreviewTilemap.SetTile(floor.GetPosition(), greenTile);

        // }

    }

    public void DeleteMouseoverEffect() {
        Vector3Int currentCell = buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        buildingPreviewTilemap.ClearAllTiles();
        buildingBasePreviewTilemap.ClearAllTiles();
        if (mouseIsHeld){
            HashSet<Vector3Int> deleteArea = GetAllCoordinatesInArea(currentCell, mousePositionWhenHoldStarted);
            foreach(Vector3Int cell in deleteArea) buildingPreviewTilemap.SetTile(cell, redTile);
        }else buildingPreviewTilemap.SetTile(currentCell, redTile);
    }

    public void HandleMouseMove() {
        BuildingController buildingController = GetBuildingController();
        buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        switch (buildingController.GetCurrentAction()) {
            case Actions.PLACE:
                PlaceMouseoverEffect();
                break;
            case Actions.DELETE:
                DeleteMouseoverEffect();
                break;
            case Actions.EDIT:
                EditMouseoverEffect();
                break;
        }
    }

    public void HandleMouseLeftClick() {
        if(EventSystem.current.IsPointerOverGameObject()) return;
        Vector3Int currentCell = buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        switch (buildingController.GetCurrentAction()){
            case Actions.PLACE:
                if (!(buildingController.GetCurrentBuilding() is Floor)) buildingController.PlaceCurrentlySelectedBuilding(currentCell);
                // else{
                //     HashSet<Vector3Int> placeFloorArea = GetAllCoordinatesInArea(currentCell, mousePositionWhenHoldStarted);
                //     foreach (Vector3Int cell in placeFloorArea) buildingController.PlaceCurrentlySelectedBuilding(cell);
                // }
                break;
            case Actions.DELETE:
                HashSet<Vector3Int> deleteArea = GetAllCoordinatesInArea(currentCell, mousePositionWhenHoldStarted);
                foreach (Vector3Int cell in deleteArea) buildingController.DeleteBuilding(cell);
                break;
            case Actions.EDIT:
                buildingController.PickupBuilding(currentCell);
                break;
        }
    }
}
