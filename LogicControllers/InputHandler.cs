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
using UnityEngine.U2D;

public class InputHandler : MonoBehaviour {

    public enum CursorType {
        Default,
        Delete,
        Place,
        Pickup,
        PlaceWallpaper,
        PlaceFlooring
    }

    [Serializable]
    private class CursorTexture {
        [SerializeField] private Actions action;
        [SerializeField] private Texture2D texture;

        public Actions Action { get => action; }
        public Texture2D Texture { get => texture; }
    }

    public static InputHandler Instance { get; private set; }

    // public bool IsSearching { get; set; } = false;
    bool mouseIsHeld = false;
    Vector3Int mousePositionWhenHoldStarted;
    Building hoveredBuilding;
    GameObject followCursorObject;
    Vector3Int mouseTileOnPreviousFrame;
    public bool keybindsShouldRegister = true;
    [SerializeField] private List<CursorTexture> cursorTextures;
    [SerializeField] private GameObject wallpaperButtonPrefab;
    [SerializeField] private GameObject flooringButtonPrefab;

    void Start() {
        if (Instance == null) Instance = this;
        else Destroy(this);

        BuildingController.SetCurrentAction(Actions.PLACE);
        mouseTileOnPreviousFrame = GetMousePositionInTilemap();

        var saveLoad = GetSettingsModal().transform.Find("TabContent").Find("General").Find("ScrollArea").Find("Content").Find("SaveLoad");
        saveLoad.Find("Save").GetComponent<Button>().onClick.AddListener(() => BuildingSaverLoader.SaveToFile());
        saveLoad.Find("Load").GetComponent<Button>().onClick.AddListener(() => BuildingSaverLoader.LoadFromFile());

        var quit = GetSettingsModal().transform.Find("TabContent").Find("Quit");
        quit.Find("QUIT").GetComponent<Button>().onClick.AddListener(() => BuildingController.QuitApp());
        quit.Find("SAVE AND QUIT").GetComponent<Button>().onClick.AddListener(() => BuildingController.SaveAndQuit());
    }

    void Update() {

        HandleMouseActions();

        // if (IsSearching) return;
        if (!keybindsShouldRegister) return;

        if (Input.GetKeyDown(KeyCode.F12)) DebugCoordinates.ToggleDebugMode();

        if (Input.GetKeyDown(KeyCode.F11)) CameraController.Instance.ToggleFullscren();

        if (KeybindsForActionArePressed(KeybindHandler.Action.Settings)) {
            if (MoveablePanel.FullFocusPanelIsOpen.Item1) MoveablePanel.FullFocusPanelIsOpen.Item2.SetPanelToClosedPosition();
            else GetSettingsModal().GetComponent<MoveablePanel>().TogglePanel();
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Quit)) {
            GameObject quitConfirmPanel = GameObject.FindGameObjectWithTag("QuitConfirm");
            quitConfirmPanel.GetComponent<MoveablePanel>().SetPanelToOpenPosition();
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Undo)) UndoRedoController.UndoLastAction();

        if (KeybindsForActionArePressed(KeybindHandler.Action.Redo)) UndoRedoController.RedoLastUndo();

        if (KeybindsForActionArePressed(KeybindHandler.Action.ToggleUnavailableTiles)) {
            InvalidTilesManager.Instance.ToggleMapUnavailableCoordinates();
            NotificationManager.Instance.SendNotification("Toggled unavailable coordinates visibility", NotificationManager.Icons.InfoIcon);
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.TogglePlantableTiles)) {
            InvalidTilesManager.Instance.ToggleMapPlantableCoordinates();
            NotificationManager.Instance.SendNotification("Toggled plantable coordinates visibility", NotificationManager.Icons.InfoIcon);
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Save)) BuildingSaverLoader.SaveToFile();

        if (KeybindsForActionArePressed(KeybindHandler.Action.Load)) BuildingSaverLoader.LoadFromFile();

        if (KeybindsForActionArePressed(KeybindHandler.Action.Place)) {
            BuildingController.SetCurrentAction(Actions.PLACE);
            NotificationManager.Instance.SendNotification("Set mode to placement", NotificationManager.Icons.InfoIcon);
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Edit)) {
            BuildingController.SetCurrentAction(Actions.EDIT);
            NotificationManager.Instance.SendNotification("Set mode to edit", NotificationManager.Icons.InfoIcon);
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.Delete)) {
            BuildingController.SetCurrentAction(Actions.DELETE);
            NotificationManager.Instance.SendNotification("Set mode to delete", NotificationManager.Icons.InfoIcon);
        }

        if (KeybindsForActionArePressed(KeybindHandler.Action.DeleteAll)) GameObject.FindGameObjectWithTag("ConfirmDeleteAll").GetComponent<MoveablePanel>().SetPanelToOpenPosition();

        if (KeybindsForActionArePressed(KeybindHandler.Action.OpenBuildingMenu)) GameObject.Find("BuildingSelect").GetComponent<MoveablePanel>().TogglePanel();

        if (KeybindsForActionArePressed(KeybindHandler.Action.OpenTotalCost)) GameObject.Find("TotalMaterialsNeeded").GetComponent<MoveablePanel>().TogglePanel();

        if (KeybindsForActionArePressed(KeybindHandler.Action.ToggleUI)) {
            SceneManager.GetSceneByName("PermanentScene").GetRootGameObjects().First(gameObject => gameObject.name == "Canvas").SetActive(!SceneManager.GetSceneByName("PermanentScene").GetRootGameObjects().First(gameObject => gameObject.name == "Canvas").activeSelf);
            NotificationManager.Instance.DestroyAllNotifications();
        }

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
        Texture2D cursorTexture = cursorTextures.First(cursorTexture => cursorTexture.Action == action).Texture;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        if (action == Actions.PLACE_WALLPAPER) {
            followCursorObject = Instantiate(wallpaperButtonPrefab, GetCanvasGameObject().transform);
            followCursorObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = WallsComponent.SelectedWallpaperSprite;
        }
        else if (action == Actions.PLACE_FLOORING) {
            followCursorObject = Instantiate(flooringButtonPrefab, GetCanvasGameObject().transform);
            followCursorObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = FlooringComponent.SelectedFloorSprite;
        }

        if (followCursorObject != null) StartCoroutine(MakeGameObjectFollowCursorButtomRight(followCursorObject));
    }

    private IEnumerator MakeGameObjectFollowCursorButtomRight(GameObject gameObjectFollowing) {
        if (gameObjectFollowing == null) yield break;
        followCursorObject.GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 0.75f);
        while (gameObjectFollowing != null) {
            gameObjectFollowing.GetComponent<RectTransform>().position = Input.mousePosition - new Vector3(-48, 48);
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
                            UndoRedoController.ignoreAction = true;
                            List<BuildingData> buildingsCreatedData = new();
                            foreach (Vector3Int position in mouseCoverageArea) {
                                BuildingController.CurrentBuildingBeingPlaced.PlaceBuilding(position);
                                buildingsCreatedData.Add(BuildingController.LastBuildingPlaced.GetComponent<BuildingSaverLoader>().SaveBuilding());
                            }
                            UndoRedoController.ignoreAction = false;
                            UndoRedoController.AddActionToLog(new UserAction(Actions.PLACE, buildingsCreatedData));
                        }
                        break;
                    case Actions.EDIT:
                        building = BuildingController.buildings.FirstOrDefault(building => building.BaseCoordinates.Contains(mousePosition) || mousePosition == building.Base);
                        if (building != null) building.PickupBuilding();
                        break;
                    case Actions.DELETE:
                        mouseCoverageArea = GetAllCoordinatesInArea(mousePositionWhenHoldStarted, mousePosition).ToArray();
                        Building[] buildings = BuildingController.buildings.Where(building => building.BaseCoordinates.Intersect(mouseCoverageArea).Count() > 0 || mouseCoverageArea.Contains(building.Base)).ToArray();
                        UndoRedoController.ignoreAction = true;
                        if (buildings.Length == 0) return;
                        List<BuildingData> buildingsDeletedData = new();
                        foreach (Building buildingToDelete in buildings) {
                            buildingsDeletedData.Add(buildingToDelete.GetComponent<BuildingSaverLoader>().SaveBuilding());
                            buildingToDelete.DeleteBuilding();
                        }
                        UndoRedoController.ignoreAction = false;
                        UndoRedoController.AddActionToLog(new UserAction(Actions.DELETE, buildingsDeletedData));
                        break;
                    case Actions.PLACE_WALLPAPER:
                        if (BuildingController.isInsideBuilding.Key) {
                            WallsComponent component = BuildingController.isInsideBuilding.Value.gameObject.GetComponent<WallsComponent>();
                            Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            Vector3Int mousePositionInInteriorTilemap = component.wallPaperTilemap.WorldToCell(mousePositionInWorld);
                            mousePositionInInteriorTilemap.z = 0;
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

        if (mouseTileOnPreviousFrame == mousePosition) return;
        mouseTileOnPreviousFrame = mousePosition;

        // //Every Frame
        if (BuildingController.CurrentAction == Actions.PLACE) {
            building = BuildingController.CurrentBuildingBeingPlaced;
            if (building != null) building.PlaceBuildingPreview(mousePosition);
            if (mouseIsHeld && BuildingController.CurrentBuildingBeingPlaced.CanBeMassPlaced) {
                mouseCoverageArea = GetAllCoordinatesInArea(mousePositionWhenHoldStarted, mousePosition).ToArray();
                Tilemap massDeleteTilemap = SceneManager.GetSceneByName("PermanentScene").GetRootGameObjects()[3].transform.Find("MassDeletePreview").GetComponent<Tilemap>();
                massDeleteTilemap.ClearAllTiles();
                if (mouseCoverageArea.Count() > 2) foreach (Vector3Int position in mouseCoverageArea) massDeleteTilemap.SetTile(position, LoadTile(Utility.Tiles.Green));
            }
        }
        else if (BuildingController.CurrentAction == Actions.DELETE) {
            if (mouseIsHeld) {
                mouseCoverageArea = GetAllCoordinatesInArea(mousePositionWhenHoldStarted, mousePosition).ToArray();
                Tilemap massDeleteTilemap = SceneManager.GetSceneByName("PermanentScene").GetRootGameObjects()[3].transform.Find("MassDeletePreview").GetComponent<Tilemap>();
                massDeleteTilemap.ClearAllTiles();
                if (mouseCoverageArea.Count() > 2) foreach (Vector3Int position in mouseCoverageArea) massDeleteTilemap.SetTile(position, LoadTile(Utility.Tiles.Red));
            }
        }

        //Mouse Hover

        // //Hover Exit
        bool isHoveredBuildingStillUnderMouse = !hoveredBuilding?.BaseCoordinates?.Contains(mousePosition) ?? false;
        if (hoveredBuilding != null && isHoveredBuildingStillUnderMouse && !BuildingController.isInsideBuilding.Key) {
            if (hoveredBuilding.CurrentBuildingState == Building.BuildingState.PLACED) hoveredBuilding.NoPreview();
            hoveredBuilding = null;
        }

        // //Hover Enter
        Building BuildingUnderMouse = BuildingController.buildings.FirstOrDefault(b => b.BaseCoordinates.Contains(mousePosition));
        bool isInsideBuilding = BuildingController.isInsideBuilding.Key;
        if (BuildingUnderMouse != null && !isInsideBuilding) {
            hoveredBuilding = BuildingController.buildings.FirstOrDefault(b => b.BaseCoordinates.Contains(mousePosition));
            hoveredBuilding.DoBuildingPreview();
        }

    }
}
