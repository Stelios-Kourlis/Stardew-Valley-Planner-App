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
using System.Diagnostics.Eventing.Reader;

public class InputHandler : MonoBehaviour {

    public enum CursorType {
        Default,
        Delete,
        Place,
        Pickup
    }

    public bool IsSearching {get; set;} = false;
    BuildingController buildingController;
    //bool mouseIsHeld = false;
    Vector3Int mousePositionWhenHoldStarted;

    void Start() {
        buildingController = GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingController>();
        SetCursor(CursorType.Default);
    }

    void Update(){
        //Debug.Log(buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        
        if (KeybindsForActionArePressed(KeybindHandler.Action.Quit)){
            GameObject quitConfirmPanel = GameObject.FindGameObjectWithTag("QuitConfirm");
            quitConfirmPanel.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Settings)) GetSettingsModalController().TogglePanel();

        if (IsSearching) return; //the 2 above should always be available, rest should be disabled when searching

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            //mouseIsHeld = true; 
            mousePositionWhenHoldStarted = buildingController.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)){
            //mouseIsHeld = false;
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Undo)) buildingController.UndoLastAction();

        if (KeybindsForActionArePressed(KeybindHandler.Action.Redo)) buildingController.RedoLastUndo();

        if (KeybindsForActionArePressed(KeybindHandler.Action.ToggleUnavailableTiles)){
            GetMapController().ToggleMapUnavailableCoordinates();
            GetNotificationManager().SendNotification("Toggled unavailable coordinates visibility");
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.TogglePlantableTiles)){
            GetMapController().ToggleMapPlantableCoordinates();
            GetNotificationManager().SendNotification("Toggled plantable coordinates visibility");
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Save)) buildingController.Save();

        if (KeybindsForActionArePressed(KeybindHandler.Action.Load)) buildingController.Load();

        if (KeybindsForActionArePressed(KeybindHandler.Action.Place)){
            Building.CurrentAction = Actions.PLACE;
            GetNotificationManager().SendNotification("Set mode to placement");
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Edit)){
            Building.CurrentAction = Actions.EDIT;
            GetNotificationManager().SendNotification("Set mode to edit");
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Delete)){
            Building.CurrentAction = Actions.DELETE;
            GetNotificationManager().SendNotification("Set mode to delete");
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.DeleteAll)) GameObject.FindGameObjectWithTag("DeleteAllButton").GetComponent<ConfirmationWidow>().OpenConfirmDialog();
    }

    public bool KeybindsForActionArePressed(KeybindHandler.Action action){//todo add priority, if you press Ctrl + D, it should not trigger D as well
        KeybindHandler.Keybind keybind = KeybindHandler.GetKeybind(action);
        foreach (KeybindHandler.Action possibleAction in Enum.GetValues(typeof(KeybindHandler.Action))){
            if (possibleAction == action) continue;
            KeybindHandler.Keybind keybindForAction = KeybindHandler.GetKeybind(possibleAction);
            if (keybindForAction.keybind == keybind.keybind && keybindForAction.optionalSecondButton != KeyCode.None && Input.GetKey(keybindForAction.optionalSecondButton)) return false;
        }
        bool isPrimaryPressed = Input.GetKeyUp(keybind.keybind);
        bool isSecondaryPressed = keybind.optionalSecondButton == KeyCode.None || Input.GetKey(keybind.optionalSecondButton);
        return isPrimaryPressed && isSecondaryPressed;
    }

    public void SetCursor(CursorType type){
        switch (type){
            case CursorType.Default:
                Cursor.SetCursor(Resources.Load("UI/Cursor") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case CursorType.Delete:
                Cursor.SetCursor(Resources.Load("UI/CursorDelete") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case CursorType.Place:
                Cursor.SetCursor(Resources.Load("UI/CursorPlace") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case CursorType.Pickup:
                Cursor.SetCursor(Resources.Load("UI/CursorHand") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
                break;
        }
    }
}
