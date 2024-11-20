using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.SpriteManager;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using static Utility.InvalidTileLoader;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using static BuildingData;
using static FlooringComponent;
using static WallsComponent;

[Serializable]
public class WallsPerTier {
    public int tier;
    public WallOrigin[] wallOrigins;
}

[Serializable]
public class FlooringPerTier {
    public int tier;
    public FlooringOrigin[] floorOrigins;
}


[RequireComponent(typeof(Building))]
public class EnterableBuildingComponent : BuildingComponent {
    public GameObject BuildingInterior { get; private set; }
    public Sprite interiorSprite;
    public Vector3Int[] InteriorAreaCoordinates { get; private set; }
    [field: SerializeField] public SpecialCoordinatesCollection InteriorSpecialTiles { get; private set; }
    public static Action EnteredOrExitedAnyBuilding { get; set; }
    public HashSet<ButtonTypes> InteriorInteractions { get; private set; } = new();
    // public GameObject button { get; private set; }
    public List<GameObject> interiorButtons;
    private static int numberOfInteriors = 0;
    private Vector3 cameraPositionBeforeEnter;
    private Scene BuildingInteriorScene;
    private InteractableBuildingComponent InteractableBuildingComponent => gameObject.GetComponent<InteractableBuildingComponent>();
    private Scene MapScene => GetMapController().MapScene;
    public int Tier => GetComponent<TieredBuildingComponent>().Tier;
    private Transform mapTransform;
    public Action InteriorUpdated { get; set; }
    public Action EnteredOrExitedBuilding { get; set; }
    public GameObject interiorSceneCanvas;
    public Action<ButtonTypes> interiorButtonClicked;

    WallsPerTier[] wallsValues;
    FlooringPerTier[] floorsValues;

    public void OnDestroy() {
        if (BuildingInteriorScene.name != null) SceneManager.UnloadSceneAsync(BuildingInteriorScene);
    }

    public void AddToInteriorUnavailableCoordinates(IEnumerable<Vector3Int> coordinates, string identifier) {
        InteriorSpecialTiles.AddSpecialTileSet(new(identifier, coordinates.ToHashSet(), TileType.Invalid));
        InvalidTilesManager.Instance.UpdateAllCoordinates();
    }

    public void RemoveFromInteriorUnavailableCoordinates(string identifier) {
        InteriorSpecialTiles.RemoveSpecialTileSet(identifier);
        InvalidTilesManager.Instance.UpdateAllCoordinates();
    }



    public void AddBuildingInterior() {

        static GameObject AddGrid() {
            GameObject grid = new("Grid");
            grid.SetActive(false);
            grid.AddComponent<Grid>();
            grid.AddComponent<Tilemap>();
            return grid;
        }

        static GameObject AddCanvas() {
            GameObject canvas = new("Canvas");
            canvas.SetActive(false);
            canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvas.GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
            canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            canvas.GetComponent<CanvasScaler>().referencePixelsPerUnit = 16;
            canvas.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        void AddInteriorWalls() {
            GameObject walls = new("Walls");
            walls.transform.SetParent(BuildingInterior.transform);
            walls.AddComponent<Tilemap>();
            walls.AddComponent<TilemapRenderer>().sortingOrder = -101;
            gameObject.AddComponent<WallsComponent>().SetWalls(wallsValues.First(val => val.tier == Tier).wallOrigins.ToList(), walls.GetComponent<Tilemap>());
        }

        void AddInteriorFlooring() {
            GameObject floors = new("Floors");
            floors.transform.SetParent(BuildingInterior.transform);
            floors.AddComponent<Tilemap>();
            floors.AddComponent<TilemapRenderer>().sortingOrder = -102;
            gameObject.AddComponent<FlooringComponent>().SetFloors(floorsValues.First(val => val.tier == Tier).floorOrigins.ToList(), floors.GetComponent<Tilemap>());
        }

        static GameObject CreateButtonGameObject() {
            GameObject button = new("button");
            button.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            button.AddComponent<UIElement>();
            button.GetComponent<UIElement>().ExpandOnHover = true;
            button.GetComponent<UIElement>().playSounds = true;
            button.GetComponent<UIElement>().SetActionToNothingOnEnter = true;
            return button;
        }

        if (BuildingInteriorScene.name != null) return; //failsafe
        BuildingInteriorScene = SceneManager.CreateScene($"BuildingInterior{numberOfInteriors++} ({Building.BuildingName})");
        interiorSprite = Resources.Load<Sprite>($"BuildingInsides/{InteractableBuildingComponent.GetBuildingInsideSpriteName()}");
        BuildingInterior = new GameObject($"{Building.BuildingName} Interior");
        InteriorAreaCoordinates = GetRectAreaFromPoint(Vector3Int.zero, (int)interiorSprite.textureRect.height / 16, (int)interiorSprite.textureRect.width / 16).ToArray();
        BuildingInterior.AddComponent<Tilemap>().SetTiles(InteriorAreaCoordinates, SplitSprite(interiorSprite));
        BuildingInterior.GetComponent<Tilemap>().CompressBounds();
        BuildingInterior.AddComponent<TilemapRenderer>().sortingOrder = -100;

        string BuildingName = GetComponent<InteractableBuildingComponent>().GetBuildingInsideSpriteName();
        var specialCoordinates = GetSpecialCoordinateSet(BuildingName);
        specialCoordinates.AddOffset(InteriorAreaCoordinates[0]);
        InteriorSpecialTiles.AddSpecialTileSet(specialCoordinates);

        GameObject grid = AddGrid();
        BuildingInterior.transform.SetParent(grid.transform);

        interiorSceneCanvas = AddCanvas();

        if (wallsValues != null && wallsValues.Count() > 0) AddInteriorWalls();

        if (floorsValues != null && floorsValues.Count() > 0) AddInteriorFlooring();

        interiorButtons = new();
        GameObject buttonPrefab = CreateButtonGameObject();

        GameObject exitButton = HUDButtonCotroller.CreateButtonNextToOtherButton(GameObject.Find("NoBuilding").GetComponent<RectTransform>(), buttonPrefab, HUDButtonCotroller.RelativePosition.BOTTOM, interiorSceneCanvas.transform);
        exitButton.name = "ENTER";
        exitButton.AddComponent<Image>().sprite = BuildingButtonController.Instance.ButtonTypesAtlas.GetSprite($"{ButtonTypes.ENTER}");
        exitButton.AddComponent<Button>().onClick.AddListener(() => {
            Building.GetComponent<EnterableBuildingComponent>().ExitBuildingInteriorEditing();
        });
        interiorButtons.Add(exitButton);



        foreach (ButtonTypes type in InteriorInteractions) {
            RectTransform startingButton = GetCanvasGameObject().transform.Find("ToggleBuildingSelectButton").GetComponent<RectTransform>();
            GameObject button = HUDButtonCotroller.CreateButtonNextToOtherButton(startingButton, buttonPrefab, HUDButtonCotroller.RelativePosition.RIGHT, interiorSceneCanvas.transform);
            button.name = type.ToString();
            button.AddComponent<Image>().sprite = BuildingButtonController.Instance.ButtonTypesAtlas.GetSprite($"{type}");
            interiorButtons.Add(button);

            switch (type) {
                case ButtonTypes.TIER_ONE:
                    button.AddComponent<Button>().onClick.AddListener(() => {
                        Building.GetComponent<TieredBuildingComponent>().SetTier(1);
                        UpdateBuildingInterior();
                    });
                    interiorButtonClicked?.Invoke(type);
                    break;
                case ButtonTypes.TIER_TWO:
                    button.AddComponent<Button>().onClick.AddListener(() => {
                        Building.GetComponent<TieredBuildingComponent>().SetTier(2);
                        UpdateBuildingInterior();
                    });
                    interiorButtonClicked?.Invoke(type);
                    break;
                case ButtonTypes.TIER_THREE:
                    button.AddComponent<Button>().onClick.AddListener(() => {
                        Building.GetComponent<TieredBuildingComponent>().SetTier(3);
                        UpdateBuildingInterior(); ;
                    });
                    interiorButtonClicked?.Invoke(type);
                    break;
                case ButtonTypes.TIER_FOUR:
                    button.AddComponent<Button>().onClick.AddListener(() => {
                        Building.GetComponent<TieredBuildingComponent>().SetTier(4);
                        UpdateBuildingInterior();
                    });
                    interiorButtonClicked?.Invoke(type);
                    break;
                case ButtonTypes.CUSTOMIZE_HOUSE_RENOVATIONS:
                    button.AddComponent<Button>().onClick.AddListener(() => {
                        Building.GetComponent<HouseExtensionsComponent>().ToggleModificationMenu();
                    });
                    interiorButtonClicked?.Invoke(type);
                    break;
                case ButtonTypes.ADD_ANIMAL:
                    button.AddComponent<Button>().onClick.AddListener(() => {
                        Building.GetComponent<AnimalHouseComponent>().ToggleAnimalMenu();
                    });
                    interiorButtonClicked?.Invoke(type);
                    break;
                default:
                    throw new System.ArgumentException($"Invalid interior interaction {type}");
            }

            startingButton = button.GetComponent<RectTransform>();
        }


        SceneManager.MoveGameObjectToScene(grid, BuildingInteriorScene);
        SceneManager.MoveGameObjectToScene(interiorSceneCanvas, BuildingInteriorScene);

        InteriorUpdated?.Invoke();
    }

    public GameObject GetInteriorButton(ButtonTypes type) {
        return interiorButtons.Find(button => button.name == type.ToString());
    }

    public void UpdateBuildingInterior() {
        if (BuildingInteriorScene.name == null) AddBuildingInterior(); //failsafe

        Transform interiorTransform = BuildingInteriorScene.GetRootGameObjects()[0].transform.GetChild(0);
        foreach (Transform buildingGameObject in interiorTransform) {
            if (!buildingGameObject.TryGetComponent(out Building building)) continue;
            if (building.CurrentBuildingState == Building.BuildingState.PLACED) building.DeleteBuilding();
        }

        BuildingInterior.GetComponent<Tilemap>().ClearAllTiles();
        interiorSprite = Resources.Load<Sprite>($"BuildingInsides/{InteractableBuildingComponent.GetBuildingInsideSpriteName()}");
        InteriorAreaCoordinates = GetRectAreaFromPoint(Vector3Int.zero, (int)interiorSprite.textureRect.height / 16, (int)interiorSprite.textureRect.width / 16).ToArray();
        BuildingInterior.GetComponent<Tilemap>().SetTiles(InteriorAreaCoordinates, SplitSprite(interiorSprite));
        BuildingInterior.GetComponent<Tilemap>().CompressBounds();
        GetCamera().GetComponent<CameraController>().UpdateTilemapBounds();
        InteriorSpecialTiles.ClearAll();
        string BuildingName = GetComponent<InteractableBuildingComponent>().GetBuildingInsideSpriteName();
        var specialCoordinates = GetSpecialCoordinateSet(BuildingName);
        specialCoordinates.AddOffset(InteriorAreaCoordinates[0]);
        InteriorSpecialTiles.AddSpecialTileSet(specialCoordinates);

        if (wallsValues != null && wallsValues.Count() > 0) gameObject.GetComponent<WallsComponent>().UpdateWalls(wallsValues.First(val => val.tier == Tier).wallOrigins.ToList());

        if (floorsValues != null && floorsValues.Count() > 0) gameObject.GetComponent<FlooringComponent>().UpdateFloors(floorsValues.First(val => val.tier == Tier).floorOrigins.ToList());

        InvalidTilesManager.Instance.UpdateAllCoordinates();

        InteriorUpdated?.Invoke();
    }

    public void ToggleEditBuildingInterior() {
        if (BuildingInterior == null) AddBuildingInterior(); //failsafe

        if (interiorSprite.name != InteractableBuildingComponent.GetBuildingInsideSpriteName()) { //In case interior need to be updates
            UpdateBuildingInterior();
        }

        if (BuildingController.isInsideBuilding.Key) ExitBuildingInteriorEditing();
        else EditBuildingInterior();
    }

    public void EditBuildingInterior() {
        BuildingController.isInsideBuilding = new KeyValuePair<bool, EnterableBuildingComponent>(true, this);

        GetComponent<InteractableBuildingComponent>().BuildingWasPlaced(); //if the buttons havent been added yet. add them

        foreach (GameObject obj in BuildingInteriorScene.GetRootGameObjects()) {
            obj.SetActive(true);
        }

        cameraPositionBeforeEnter = GetCamera().transform.position;
        GetCamera().GetComponent<CameraController>().SetPosition(new Vector3(BuildingInterior.GetComponent<Tilemap>().size.x / 2, BuildingInterior.GetComponent<Tilemap>().size.x / 2)); //center the camera on the interior

        foreach (ButtonTypes type in InteractableBuildingComponent.BuildingInteractions) {
            GameObject button = InteractableBuildingComponent.ButtonParentGameObject.transform.Find(type.ToString()).gameObject;
            if (type == ButtonTypes.ENTER) {
                button.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                button.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
                button.GetComponent<RectTransform>().position = new Vector3(Screen.width - 60, 60, 0);
            }
            else button.SetActive(false);
        }

        foreach (GameObject obj in MapScene.GetRootGameObjects()) {
            obj.SetActive(false);
        }

        foreach (Transform obj in GetCanvasGameObject().transform) {
            if (obj.name.Contains("InteractionButtons")) obj.gameObject.SetActive(false);
        }

        mapTransform = BuildingController.CurrentTilemapTransform;
        BuildingController.SetCurrentTilemapTransform(BuildingInterior.transform);

        SceneManager.SetActiveScene(BuildingInteriorScene);

        EnteredOrExitedAnyBuilding?.Invoke();
        EnteredOrExitedBuilding?.Invoke();
        BuildingController.LastBuildingObjectCreated.transform.SetParent(BuildingInterior.transform);
    }

    public void ExitBuildingInteriorEditing() {
        BuildingController.isInsideBuilding = new KeyValuePair<bool, EnterableBuildingComponent>(false, null);

        foreach (GameObject obj in BuildingInteriorScene.GetRootGameObjects()) {
            obj.SetActive(false);
        }

        GetCamera().GetComponent<CameraController>().SetPosition(cameraPositionBeforeEnter); //center the camera on the interior
        // SceneManager.MoveGameObjectToScene(GetCamera(), MapScene);

        foreach (ButtonTypes type in InteractableBuildingComponent.BuildingInteractions) {
            GameObject button = InteractableBuildingComponent.ButtonParentGameObject.transform.Find(type.ToString()).gameObject;
            if (type == ButtonTypes.ENTER) button.GetComponent<RectTransform>().sizeDelta = new Vector2(75, 75);
            InteractableBuildingComponent.ButtonParentGameObject.transform.Find(type.ToString()).gameObject.SetActive(true);
        }

        for (int i = 0; i < Building.Transform.parent.childCount; i++) {
            if (Building.Transform.parent.GetChild(i).gameObject == Building.Transform.gameObject) Building.Transform.gameObject.GetComponent<Tilemap>().color = new Color(1, 1, 1, 1); //make building transparent
            else Building.Transform.parent.GetChild(i).gameObject.SetActive(true); //disable all other buildings
        }

        foreach (GameObject obj in MapScene.GetRootGameObjects()) {
            obj.SetActive(true);
        }

        BuildingController.SetCurrentTilemapTransform(mapTransform);

        SceneManager.SetActiveScene(MapScene);

        EnteredOrExitedAnyBuilding?.Invoke();
        EnteredOrExitedBuilding?.Invoke();
        BuildingController.LastBuildingObjectCreated.transform.SetParent(GetGridTilemap().transform);

    }

    public override void Load(BuildingScriptableObject bso) {
        InteriorInteractions = bso.interiorInteractions.ToHashSet();
        // InteriorInteractions.Add(ButtonTypes.ENTER);

        floorsValues = bso.interiorFlooring;
        wallsValues = bso.interiorWalls;

        Building.BuildingPlaced += _ => AddBuildingInterior();
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.ENTER);

        InteriorSpecialTiles = new SpecialCoordinatesCollection();
    }

    public override ComponentData Save() {
        if (BuildingInteriorScene.name == null) return null; //if interior is null, dont save

        ComponentData data = new(typeof(EnterableBuildingComponent));
        int index = 0;
        foreach (Transform child in BuildingInteriorScene.GetRootGameObjects()[0].transform.GetChild(0)) {
            if (child.TryGetComponent(out Building building)) {
                if (building.CurrentBuildingState != Building.BuildingState.PLACED) continue; //only save placed buildings
                JProperty jproperty = new(index.ToString(), BuildingSaverLoader.Instance.SaveBuilding(building).ToJson());
                data.AddProperty(jproperty);
                index++;
            }
        }
        return data;

    }

    public override void Load(ComponentData data) {
        // return;
        UpdateBuildingInterior();
        EditBuildingInterior();
        foreach (JProperty property in data.GetAllComponentDataProperties()) {
            BuildingSaverLoader.Instance.LoadBuilding(BuildingSaverLoader.Instance.ParseBuildingFromJson(property));
        }
        ExitBuildingInteriorEditing();
    }
}
