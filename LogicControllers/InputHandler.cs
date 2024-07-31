using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;
using static Utility.BuildingManager;
using static Utility.ClassManager;
using static Utility.SpriteManager;
using System;
using System.Diagnostics.Eventing.Reader;

public class InputHandler : MonoBehaviour {

    public enum CursorType {
        Default,
        Delete,
        Place,
        Pickup
    }

    public bool IsSearching { get; set; } = false;
    bool mouseIsHeld = false;
    Vector3Int mousePositionWhenHoldStarted;
    Building hoveredBuilding;

    void Start() {
        SetCursor(CursorType.Default);
    }

    void Update() {

        HandleMouseActions();

        if (IsSearching) return;

        if (KeybindsForActionArePressed(KeybindHandler.Action.Settings)) GetSettingsModal().GetComponent<MoveablePanel>().TogglePanel();

        if (KeybindsForActionArePressed(KeybindHandler.Action.Quit)) {
            GameObject quitConfirmPanel = GameObject.FindGameObjectWithTag("QuitConfirm");
            quitConfirmPanel.GetComponent<MoveablePanel>().SetPanelToOpenPosition();
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Undo)) UndoRedoController.UndoLastAction();

        if (KeybindsForActionArePressed(KeybindHandler.Action.Redo)) UndoRedoController.RedoLastUndo();

        if (KeybindsForActionArePressed(KeybindHandler.Action.ToggleUnavailableTiles)) {
            GetMapController().ToggleMapUnavailableCoordinates();
            GetNotificationManager().SendNotification("Toggled unavailable coordinates visibility", NotificationManager.Icons.InfoIcon);
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.TogglePlantableTiles)) {
            GetMapController().ToggleMapPlantableCoordinates();
            GetNotificationManager().SendNotification("Toggled plantable coordinates visibility", NotificationManager.Icons.InfoIcon);
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Save)) BuildingController.Save();

        if (KeybindsForActionArePressed(KeybindHandler.Action.Load)) BuildingController.Load();

        if (KeybindsForActionArePressed(KeybindHandler.Action.Place)) {
            BuildingController.SetCurrentAction(Actions.PLACE);
            GetNotificationManager().SendNotification("Set mode to placement", NotificationManager.Icons.InfoIcon);
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Edit)) {
            BuildingController.SetCurrentAction(Actions.EDIT);
            GetNotificationManager().SendNotification("Set mode to edit", NotificationManager.Icons.InfoIcon);
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Delete)) {
            BuildingController.SetCurrentAction(Actions.DELETE);
            GetNotificationManager().SendNotification("Set mode to delete", NotificationManager.Icons.InfoIcon);
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.DeleteAll)) GameObject.FindGameObjectWithTag("ConfirmDeleteAll").GetComponent<MoveablePanel>().SetPanelToOpenPosition();
    }

    public bool KeybindsForActionArePressed(KeybindHandler.Action action) {
        KeybindHandler.Keybind keybind = KeybindHandler.GetKeybind(action);
        foreach (KeybindHandler.Action possibleAction in Enum.GetValues(typeof(KeybindHandler.Action))) {
            if (possibleAction == action) continue;
            KeybindHandler.Keybind keybindForAction = KeybindHandler.GetKeybind(possibleAction);
            if (keybindForAction.keybind == keybind.keybind && keybindForAction.optionalSecondButton != KeyCode.None && Input.GetKey(keybindForAction.optionalSecondButton)) return false;
        }
        bool isPrimaryPressed = Input.GetKeyUp(keybind.keybind);
        bool isSecondaryPressed = keybind.optionalSecondButton == KeyCode.None || Input.GetKey(keybind.optionalSecondButton);
        return isPrimaryPressed && isSecondaryPressed;
    }

    public void SetCursor(CursorType type) {
        switch (type) {
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

    private void HandleMouseActions() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            mouseIsHeld = true;
            mousePositionWhenHoldStarted = GetMousePositionInTilemap();
        }

        Vector3Int mousePosition = GetMousePositionInTilemap();
        Building building;
        Vector3Int[] mouseCoverageArea;

        //Mouse Left Click
        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            mouseIsHeld = false;
            GetGridTilemap().gameObject.transform.Find("MassDeletePreview").GetComponent<Tilemap>().ClearAllTiles();
            if (LeftClickShouldRegister()) {
                switch (BuildingController.CurrentAction) {
                    case Actions.PLACE:
                        if (!BuildingController.CurrentBuildingBeingPlaced.CanBeMassPlaced) BuildingController.CurrentBuildingBeingPlaced.PlaceBuilding(mousePosition);
                        else {
                            mouseCoverageArea = GetAllCoordinatesInArea(mousePositionWhenHoldStarted, mousePosition).ToArray();
                            foreach (Vector3Int position in mouseCoverageArea) BuildingController.CurrentBuildingBeingPlaced.PlaceBuilding(position);
                        }
                        break;
                    case Actions.EDIT:
                        building = BuildingController.buildings.FirstOrDefault(building => building.BaseCoordinates.Contains(mousePosition));
                        if (building != null) building.PickupBuilding();
                        break;
                    case Actions.DELETE:
                        mouseCoverageArea = GetAllCoordinatesInArea(mousePositionWhenHoldStarted, mousePosition).ToArray();
                        Building[] buildings = BuildingController.buildings.Where(building => building.BaseCoordinates.Intersect(mouseCoverageArea).Count() > 0).ToArray();
                        foreach (Building buildingToDelete in buildings) buildingToDelete.DeleteBuilding();
                        break;
                }
            }
        }

        //Mouse Right Click
        if (Input.GetKeyUp(KeyCode.Mouse1)) {
            building = BuildingController.buildings.FirstOrDefault(building => building.BaseCoordinates.Contains(mousePosition));
            if (building != null)
                if (building.TryGetComponent(out InteractableBuildingComponent interactableBuildingComponent)) interactableBuildingComponent.OnMouseRightClick();
        }

        // //Every Frame
        if (BuildingController.CurrentAction == Actions.PLACE) {
            building = BuildingController.CurrentBuildingBeingPlaced;
            if (building != null) building.PlaceBuildingPreview(mousePosition);
            if (mouseIsHeld && BuildingController.CurrentBuildingBeingPlaced.CanBeMassPlaced) {
                mouseCoverageArea = GetAllCoordinatesInArea(mousePositionWhenHoldStarted, mousePosition).ToArray();
                GetGridTilemap().gameObject.transform.Find("MassDeletePreview").GetComponent<Tilemap>().ClearAllTiles();
                if (mouseCoverageArea.Count() > 2) foreach (Vector3Int position in mouseCoverageArea) GetGridTilemap().gameObject.transform.Find("MassDeletePreview").GetComponent<Tilemap>().SetTile(position, LoadTile("GreenTile"));
            }
        }

        //Mouse Hover

        // //Hover Exit //todo fix hovering
        // bool isHoveredBuildingStillUnderMouse = !hoveredBuilding?.BaseCoordinates?.Contains(mousePosition) ?? false;
        // if (hoveredBuilding != null && isHoveredBuildingStillUnderMouse && !BuildingController.isInsideBuilding.Key) {
        //     if (hoveredBuilding.TryGetComponent(out InteractableBuildingComponent interactableBuildingComponent)) {
        //         interactableBuildingComponent.OnMouseExit();
        //     }
        //     if (hoveredBuilding.IsPlaced) hoveredBuilding.StopBuildingPreview();
        //     hoveredBuilding = null;
        // }

        // //Hover Enter
        // Building BuildingUnderMouse = BuildingController.buildings.FirstOrDefault(b => b.BaseCoordinates.Contains(mousePosition));
        // bool isInsideBuilding = BuildingController.isInsideBuilding.Key;
        // if (BuildingUnderMouse != null && !isInsideBuilding) {
        //     hoveredBuilding = BuildingController.buildings.FirstOrDefault(b => b.BaseCoordinates.Contains(mousePosition));
        //     hoveredBuilding.DoBuildingPreview();
        //     if (hoveredBuilding.TryGetComponent(out InteractableBuildingComponent interactableBuildingComponent)) {
        //         interactableBuildingComponent.OnMouseEnter();
        //     }
        // }

    }
}
