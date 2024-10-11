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
using UnityEngine.WSA;

[RequireComponent(typeof(Building))]
public class EnterableBuildingComponent : BuildingComponent {
    public GameObject BuildingInterior { get; private set; }
    public Sprite interiorSprite;
    public Vector3Int[] InteriorAreaCoordinates { get; private set; }
    [field: SerializeField] public SpecialCoordinatesCollection InteriorSpecialTiles { get; private set; }
    public static Action EnteredOrExitedBuilding { get; set; }
    public HashSet<ButtonTypes> InteriorInteractions { get; private set; } = new();
    public GameObject InteriorButtonsParent { get; private set; }
    private static int numberOfInteriors = 0;
    private Vector3 cameraPositionBeforeEnter;
    private Scene BuildingInteriorScene;
    private InteractableBuildingComponent InteractableBuildingComponent => gameObject.GetComponent<InteractableBuildingComponent>();
    private Scene MapScene => GetMapController().MapScene;
    private Transform mapTransform;

    Dictionary<int, List<WallOrigin>> wallsValues;
    Dictionary<int, List<FlooringOrigin>> floorsValues;

    public EnterableBuildingComponent AddInteriorInteractions(HashSet<ButtonTypes> interiorInteractions) {
        InteriorInteractions = interiorInteractions;
        InteriorInteractions.Add(ButtonTypes.ENTER);
        return this;
    }

    public EnterableBuildingComponent AddWalls(Dictionary<int, List<WallOrigin>> values) {
        wallsValues = values;
        return this;
    }

    public EnterableBuildingComponent AddFloors(Dictionary<int, List<FlooringOrigin>> values) {
        floorsValues = values;
        // Debug.Log(floorsValues);
        return this;
    }

    public void Awake() {
        Building.BuildingPlaced += _ => AddBuildingInterior();
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.ENTER);

        InteriorSpecialTiles = new SpecialCoordinatesCollection();
    }

    public void OnDestroy() {
        if (BuildingInteriorScene.name != null) SceneManager.UnloadSceneAsync(BuildingInteriorScene);
    }

    // public void AddToInteriorUnavailableCoordinates(Vector3Int coordinate) {
    //     InteriorUnavailableCoordinates.Add(coordinate);
    //     InvalidTilesManager.Instance.UpdateAllCoordinates();
    // }

    public void AddToInteriorUnavailableCoordinates(IEnumerable<Vector3Int> coordinates, string identifier) {
        InteriorSpecialTiles.AddSpecialTileSet(new(identifier, coordinates.ToHashSet(), TileType.Invalid));
        InvalidTilesManager.Instance.UpdateAllCoordinates();
    }

    // public void RemoveFromInteriorUnavailableCoordinates(Vector3Int coordinate) {
    //     InteriorUnavailableCoordinates.Remove(coordinate);
    //     InvalidTilesManager.Instance.UpdateAllCoordinates();
    // }

    public void RemoveFromInteriorUnavailableCoordinates(string identifier) {
        InteriorSpecialTiles.RemoveSpecialTileSet(identifier);
        InvalidTilesManager.Instance.UpdateAllCoordinates();
    }

    public void AddBuildingInterior() {
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

        GameObject grid = new("Grid");
        grid.SetActive(false);
        grid.AddComponent<Grid>();
        grid.AddComponent<Tilemap>();
        BuildingInterior.transform.SetParent(grid.transform);

        GameObject canvas = new("Canvas");
        canvas.SetActive(false);
        canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        canvas.GetComponent<CanvasScaler>().referencePixelsPerUnit = 16;
        canvas.AddComponent<GraphicRaycaster>();

        if (wallsValues != null) {
            GameObject walls = new("Walls");
            walls.transform.SetParent(BuildingInterior.transform);
            walls.AddComponent<Tilemap>();
            walls.AddComponent<TilemapRenderer>().sortingOrder = -101;
            gameObject.AddComponent<WallsComponent>().SetWalls(wallsValues[GetComponent<TieredBuildingComponent>().Tier], walls.GetComponent<Tilemap>());
        }

        if (floorsValues != null) {
            GameObject floors = new("Floors");
            floors.transform.SetParent(BuildingInterior.transform);
            floors.AddComponent<Tilemap>();
            floors.AddComponent<TilemapRenderer>().sortingOrder = -102;
            // Debug.Log(floorsValues);
            gameObject.AddComponent<FlooringComponent>().SetFloors(floorsValues[GetComponent<TieredBuildingComponent>().Tier], floors.GetComponent<Tilemap>());
        }

        if (InteriorInteractions.Count > 0) {
            InteriorButtonsParent = new("InteriorButtons");
            InteriorButtonsParent.transform.SetParent(canvas.transform);
            InteriorButtonsParent.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            InteriorButtonsParent.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            InteriorButtonsParent.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
            InteriorButtonsParent.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
            InteriorButtonsParent.GetComponent<RectTransform>().position = new Vector3(-10, -350, 0); //this puts it right below the other button
            InteriorButtonsParent.AddComponent<FoldingMenuGroup>();

            GameObject closeButton = new("Close");
            closeButton.transform.SetParent(InteriorButtonsParent.transform);
            closeButton.AddComponent<FoldingMenuItem>().isAnchorButton = true;
            closeButton.AddComponent<UIElement>();
            closeButton.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            closeButton.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            closeButton.AddComponent<Image>().sprite = BuildingButtonController.Instance.ButtonTypesAtlas.GetSprite("CloseFoldingMenu");
            closeButton.AddComponent<Button>().onClick.AddListener(() => closeButton.transform.parent.GetComponent<FoldingMenuGroup>().ToggleMenu());


            foreach (ButtonTypes type in InteriorInteractions) {
                GameObject button = new(type.ToString());
                button.transform.SetParent(InteriorButtonsParent.transform);
                button.AddComponent<FoldingMenuItem>();
                button.AddComponent<UIElement>();
                button.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                button.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                button.AddComponent<Image>().sprite = BuildingButtonController.Instance.ButtonTypesAtlas.GetSprite($"{type}");
                switch (type) {
                    case ButtonTypes.TIER_ONE:
                        button.AddComponent<Button>().onClick.AddListener(() => {
                            Building.GetComponent<TieredBuildingComponent>().SetTier(1);
                            UpdateBuildingInterior();
                        });
                        break;
                    case ButtonTypes.TIER_TWO:
                        button.AddComponent<Button>().onClick.AddListener(() => {
                            Building.GetComponent<TieredBuildingComponent>().SetTier(2);
                            UpdateBuildingInterior();
                        });
                        break;
                    case ButtonTypes.TIER_THREE:
                        button.AddComponent<Button>().onClick.AddListener(() => {
                            Building.GetComponent<TieredBuildingComponent>().SetTier(3);
                            UpdateBuildingInterior(); ;
                        });
                        break;
                    case ButtonTypes.TIER_FOUR:
                        button.AddComponent<Button>().onClick.AddListener(() => {
                            Building.GetComponent<TieredBuildingComponent>().SetTier(4);
                            UpdateBuildingInterior();
                        });
                        break;
                    case ButtonTypes.CUSTOMIZE_HOUSE_RENOVATIONS:
                        button.AddComponent<Button>().onClick.AddListener(() => {
                            Building.GetComponent<HouseExtensionsComponent>().ToggleModificationMenu();
                        });
                        break;
                    case ButtonTypes.ENTER:
                        button.AddComponent<Button>().onClick.AddListener(() => {
                            closeButton.transform.parent.GetComponent<FoldingMenuGroup>().CloseMenu();
                            Building.GetComponent<EnterableBuildingComponent>().ExitBuildingInteriorEditing();
                        });
                        break;
                    default:
                        throw new System.ArgumentException($"Invalid interior interaction {type}");
                }

                // button.GetComponent<Button>().onClick.AddListener(() => closeButton.transform.parent.GetComponent<FoldingMenuGroup>().ToggleMenu());

                closeButton.transform.SetAsLastSibling();

            }
        }



        SceneManager.MoveGameObjectToScene(grid, BuildingInteriorScene);
        SceneManager.MoveGameObjectToScene(canvas, BuildingInteriorScene);
        // var PlantableCoordinates = GetSpecialCoordinateSet(BuildingName, TileType.Plantable).Select(coordinate => coordinate + InteriorAreaCoordinates[0]);
        // InteriorSpecialTiles.AddSpecialTileSet(new SpecialCoordinateRect($"{BuildingName}Plantable", PlantableCoordinates, TileType.Plantable));

        // Debug.Log("added interior");
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

        if (wallsValues != null) gameObject.GetComponent<WallsComponent>().UpdateWalls(wallsValues[GetComponent<TieredBuildingComponent>().Tier]);

        if (floorsValues != null) gameObject.GetComponent<FlooringComponent>().UpdateFloors(floorsValues[GetComponent<TieredBuildingComponent>().Tier]);

        InvalidTilesManager.Instance.UpdateAllCoordinates();
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

        mapTransform = BuildingController.CurrentTilemapTransform;
        BuildingController.SetCurrentTilemapTransform(BuildingInterior.transform);

        SceneManager.SetActiveScene(BuildingInteriorScene);

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

        EnteredOrExitedBuilding?.Invoke();
        BuildingController.LastBuildingObjectCreated.transform.SetParent(GetGridTilemap().transform);

    }

    public override ComponentData Save() {
        if (BuildingInteriorScene.name == null) return null;

        ComponentData data = new(typeof(EnterableBuildingComponent), new());
        int index = 0;
        foreach (Transform building in BuildingInteriorScene.GetRootGameObjects()[0].transform.GetChild(0)) {
            if (building.GetComponent<Building>() == null) continue;
            if (building.GetComponent<Building>().CurrentBuildingState != Building.BuildingState.PLACED) continue; //only save placed buildings
            JProperty jproperty = new(index.ToString(), building.GetComponent<BuildingSaverLoader>().SaveBuilding().ToJson());
            data.AddProperty(jproperty);
            index++;
        }
        return data;

    }

    public override void Load(ComponentData data) {
        // AddBuildingInterior();
        UpdateBuildingInterior();
        EditBuildingInterior();
        foreach (var property in data.componentData) {
            JObject buildingData = (JObject)property.Value;
            string buildingName = buildingData.Value<string>("Building Type");
            int lowerLeftX = buildingData.Value<int>("Lower Left Corner X");
            int lowerLeftY = buildingData.Value<int>("Lower Left Corner Y");

            List<ComponentData> allComponentData = new();
            foreach (var component in buildingData.Properties().Skip(3)) {
                Type componentType = Type.GetType(component.Name);
                List<JProperty> componentData = new();
                foreach (JProperty item in component.Value.Cast<JProperty>()) componentData.Add(item);
                allComponentData.Add(new(componentType, componentData));
            }

            BuildingController.PlaceSavedBuilding(new BuildingData(Type.GetType(buildingName), new Vector3Int(lowerLeftX, lowerLeftY, 0), allComponentData.ToArray()));
        }
        ExitBuildingInteriorEditing();
    }
}
