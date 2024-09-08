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
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.CodeDom;

public class InputHandler : MonoBehaviour {

    public enum CursorType {
        Default,
        Delete,
        Place,
        Pickup,
        PlaceWallpaper,
        PlaceFlooring
    }

    public bool IsSearching { get; set; } = false;
    bool mouseIsHeld = false;
    Vector3Int mousePositionWhenHoldStarted;
    Building hoveredBuilding;
    GameObject followCursorObject;

    void Start() {
        SetCursorBasedOnCurrentAction(Actions.DO_NOTHING);
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
            MapController.ToggleMapUnavailableCoordinates();
            GetNotificationManager().SendNotification("Toggled unavailable coordinates visibility", NotificationManager.Icons.InfoIcon);
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.TogglePlantableTiles)) {
            MapController.ToggleMapPlantableCoordinates();
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

        if (KeybindsForActionArePressed(KeybindHandler.Action.OpenBuildingMenu)) GameObject.Find("BuildingSelect").GetComponent<MoveablePanel>().TogglePanel();
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

    public void SetCursorBasedOnCurrentAction(Actions action) {
        // Debug.Log("Setting cursor to " + action);
        if (followCursorObject != null) {
            StopCoroutine(MakeGameObjectFollowCursorButtomRight(followCursorObject));
            Destroy(followCursorObject);
            followCursorObject = null;
        }
        switch (action) {
            case Actions.DO_NOTHING:
                Cursor.SetCursor(Resources.Load("UI/Cursor") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case Actions.DELETE:
                Cursor.SetCursor(Resources.Load("UI/CursorDelete") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case Actions.PLACE:
                Cursor.SetCursor(Resources.Load("UI/CursorPlace") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case Actions.EDIT:
                Cursor.SetCursor(Resources.Load("UI/CursorHand") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case Actions.PLACE_WALLPAPER:
                Cursor.SetCursor(Resources.Load("UI/Cursor") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
                GameObject wallpaperPreview = Resources.Load("UI/WallpaperButton") as GameObject;
                followCursorObject = Instantiate(wallpaperPreview, GetCanvasGameObject().transform);
                followCursorObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = WallsComponent.SelectedWallpaperSprite;
                StartCoroutine(MakeGameObjectFollowCursorButtomRight(followCursorObject));
                break;
            case Actions.PLACE_FLOORING:
                Cursor.SetCursor(Resources.Load("UI/Cursor") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
                GameObject floorPreview = Resources.Load("UI/FlooringButton") as GameObject;
                followCursorObject = Instantiate(floorPreview, GetCanvasGameObject().transform);
                followCursorObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = FlooringComponent.SelectedFloorSprite;
                StartCoroutine(MakeGameObjectFollowCursorButtomRight(followCursorObject));
                break;
        }
    }

    private IEnumerator MakeGameObjectFollowCursorButtomRight(GameObject gameObjectFollowing) {
        followCursorObject.GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 0.75f);
        while (true) {
            if (gameObjectFollowing != null) gameObjectFollowing.GetComponent<RectTransform>().position = Input.mousePosition - new Vector3(-48, 48);
            yield return null;
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
            GameObject.Find("MassDeletePreview").GetComponent<Tilemap>().ClearAllTiles();
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
                    case Actions.PLACE_WALLPAPER:
                        if (BuildingController.isInsideBuilding.Key) {
                            WallsComponent component = BuildingController.isInsideBuilding.Value.gameObject.GetComponent<WallsComponent>();
                            Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            Vector3Int mousePositionInInteriorTilemap = component.wallPaperTilemap.WorldToCell(mousePositionInWorld);
                            mousePositionInInteriorTilemap.z = 0;
                            // Debug.Log("Applying to " + mousePositionInInteriorTilemap);
                            component.ApplyCurrentWallpaper(mousePosition);
                        }
                        break;
                    case Actions.PLACE_FLOORING:
                        if (BuildingController.isInsideBuilding.Key) {
                            FlooringComponent component = BuildingController.isInsideBuilding.Value.gameObject.GetComponent<FlooringComponent>();
                            Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            Vector3Int mousePositionInInteriorTilemap = component.flooringTilemap.WorldToCell(mousePositionInWorld);
                            mousePositionInInteriorTilemap.z = 0;
                            component.ApplyCurrentFloorTexture(mousePosition);
                        }
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
                Tilemap massDeleteTilemap = SceneManager.GetSceneByName("PermanentScene").GetRootGameObjects()[3].transform.Find("MassDeletePreview").GetComponent<Tilemap>();
                massDeleteTilemap.ClearAllTiles();
                if (mouseCoverageArea.Count() > 2) foreach (Vector3Int position in mouseCoverageArea) massDeleteTilemap.SetTile(position, LoadTile("GreenTile"));
            }
        }

        //Mouse Hover

        // //Hover Exit //todo fix hovering
        bool isHoveredBuildingStillUnderMouse = !hoveredBuilding?.BaseCoordinates?.Contains(mousePosition) ?? false;
        if (hoveredBuilding != null && isHoveredBuildingStillUnderMouse && !BuildingController.isInsideBuilding.Key) {
            // if (hoveredBuilding.TryGetComponent(out InteractableBuildingComponent interactableBuildingComponent)) {
            //         interactableBuildingComponent.OnMouseExit();
            //     }
            if (hoveredBuilding.CurrentBuildingState == Building.BuildingState.PLACED) hoveredBuilding.NoPreview();
            hoveredBuilding = null;
        }

        // //Hover Enter
        Building BuildingUnderMouse = BuildingController.buildings.FirstOrDefault(b => b.BaseCoordinates.Contains(mousePosition));
        bool isInsideBuilding = BuildingController.isInsideBuilding.Key;
        if (BuildingUnderMouse != null && !isInsideBuilding) {
            hoveredBuilding = BuildingController.buildings.FirstOrDefault(b => b.BaseCoordinates.Contains(mousePosition));
            hoveredBuilding.DoBuildingPreview();
            // if (hoveredBuilding.TryGetComponent(out InteractableBuildingComponent interactableBuildingComponent)) {
            //     interactableBuildingComponent.OnMouseEnter();
            // }
        }

    }
}
