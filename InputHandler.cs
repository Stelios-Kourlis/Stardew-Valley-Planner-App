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

    public bool IsSearching {get; set;} = false;
    BuildingController buildingController;
    //bool mouseIsHeld = false;
    Vector3Int mousePositionWhenHoldStarted;
    // Tilemap buildingPreviewTilemap, buildingBasePreviewTilemap;

    void Start() {
        // Sprite redTileSprite = Sprite.Create(Resources.Load("RedTile") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        // redTile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        // redTile.sprite = redTileSprite;

        // Sprite greenTileSprite = Sprite.Create(Resources.Load("GreenTile") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        // greenTile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        // greenTile.sprite = greenTileSprite;

        // GameObject[] tilemaps = GameObject.FindGameObjectsWithTag("MouseTileMap");
        // buildingPreviewTilemap = tilemaps[0].GetComponent<Tilemap>(); //the translucent version of the building
        // buildingBasePreviewTilemap = tilemaps[1].GetComponent<Tilemap>(); //the red/green tiles representing the structure's base and follow the mouse

        buildingController = GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingController>();

        Cursor.SetCursor(Resources.Load("UI/Cursor") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
    }

    void Update(){
        //Debug.Log(buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        
        //HandleMouseMove();
        if (IsSearching) return;

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            //mouseIsHeld = true; 
            mousePositionWhenHoldStarted = buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)){
            //mouseIsHeld = false;
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z) ){
            buildingController.UndoLastAction();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y) ){
            buildingController.RedoLastUndo();
        }

        if (Input.GetKeyUp(KeyCode.I)) GetMapController().ToggleMapUnavailableCoordinates();

        if (Input.GetKeyUp(KeyCode.Escape)) GameObject.FindGameObjectWithTag("SettingsButton").GetComponent<SettingsButton>().ToggleSettingsModal();

        if (Input.GetKeyUp(KeyCode.S)) buildingController.Save();

        if (Input.GetKeyUp(KeyCode.L)) buildingController.Load();

        if (Input.GetKeyUp(KeyCode.D)) GameObject.FindGameObjectWithTag("DeleteAllButton").GetComponent<ConfirmationWidow>().OpenConfirmDialog();

        if (Input.GetKeyUp(KeyCode.Q)){
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }

}
