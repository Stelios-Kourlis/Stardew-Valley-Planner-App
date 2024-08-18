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

[RequireComponent(typeof(Building))]
public class EnterableBuildingComponent : BuildingComponent {
    public GameObject BuildingInterior { get; private set; }
    public Sprite interriorSprite;
    public Vector3Int[] InteriorAreaCoordinates { get; private set; }
    public HashSet<Vector3Int> InteriorUnavailableCoordinates { get; private set; }
    public HashSet<Vector3Int> InteriorPlantableCoordinates { get; private set; }
    public static Action EnteredOrExitedBuilding { get; set; }
    public HashSet<ButtonTypes> InteriorInteractions { get; private set; } = new();
    private static int numberOfInteriors = 0;
    private Vector3 cameraPositionBeforeEnter;
    private Scene BuildingInteriorScene;
    private InteractableBuildingComponent InteractableBuildingComponent => gameObject.GetComponent<InteractableBuildingComponent>();
    private Scene MapScene => GetMapController().MapScene;
    private Transform mapTransform;

    public EnterableBuildingComponent AddInteriorInteractions(HashSet<ButtonTypes> interiorInteractions) {
        InteriorInteractions = interiorInteractions;
        return this;
    }

    public void Awake() {
        Building.BuildingPlaced += _ => AddBuildingInterior();
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.ENTER);
    }

    public void AddBuildingInterior() {
        if (BuildingInteriorScene.name != null) return; //failsafe
        BuildingInteriorScene = SceneManager.CreateScene($"BuildingInterior{numberOfInteriors++} ({Building.BuildingName})");
        interriorSprite = Resources.Load<Sprite>($"BuildingInsides/{InteractableBuildingComponent.GetBuildingInsideSpriteName()}");
        BuildingInterior = new GameObject($"{Building.BuildingName} Interior");
        InteriorAreaCoordinates = GetAreaAroundPosition(Vector3Int.zero, (int)interriorSprite.textureRect.height / 16, (int)interriorSprite.textureRect.width / 16).ToArray();
        BuildingInterior.AddComponent<Tilemap>().SetTiles(InteriorAreaCoordinates, SplitSprite(interriorSprite));
        BuildingInterior.GetComponent<Tilemap>().CompressBounds();
        BuildingInterior.AddComponent<TilemapRenderer>().sortingOrder = -100;

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

        if (InteriorInteractions.Count > 0) {
            GameObject parent = new("InteriorButtons");
            parent.transform.SetParent(canvas.transform);
            parent.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            parent.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            parent.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
            parent.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
            parent.GetComponent<RectTransform>().position = new Vector3(-10, -350, 0); //this puts it right below the other button
            parent.AddComponent<FoldingMenuGroup>();

            GameObject closeButton = new("Close");
            closeButton.transform.SetParent(parent.transform);
            closeButton.AddComponent<FoldingMenuItem>().isAnchorButton = true;
            closeButton.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
            closeButton.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            closeButton.AddComponent<Image>().sprite = Resources.Load<Sprite>("UI/CloseFoldingMenu");
            closeButton.AddComponent<Button>().onClick.AddListener(() => closeButton.transform.parent.GetComponent<FoldingMenuGroup>().ToggleMenu());


            foreach (ButtonTypes type in InteriorInteractions) {
                GameObject button = new(type.ToString());
                button.transform.SetParent(parent.transform);
                button.AddComponent<FoldingMenuItem>();
                button.AddComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
                button.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                button.AddComponent<Image>().sprite = Resources.Load<Sprite>($"UI/{type}");
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
                            Debug.Log("WIP");
                        });
                        break;
                    default:
                        throw new System.ArgumentException($"Invalid interior interaction {type}");
                }

                button.GetComponent<Button>().onClick.AddListener(() => closeButton.transform.parent.GetComponent<FoldingMenuGroup>().ToggleMenu());

                closeButton.transform.SetAsLastSibling();
            }
        }



        SceneManager.MoveGameObjectToScene(grid, BuildingInteriorScene);
        SceneManager.MoveGameObjectToScene(canvas, BuildingInteriorScene);

        string BuildingName = GetComponent<InteractableBuildingComponent>().GetBuildingInsideSpriteName();
        InteriorUnavailableCoordinates = GetInsideUnavailableCoordinates(BuildingName).Select(coordinate => coordinate + InteriorAreaCoordinates[0]).ToHashSet();
        InteriorPlantableCoordinates = GetInsidePlantableCoordinates(BuildingName).Select(coordinate => coordinate + InteriorAreaCoordinates[0]).ToHashSet();
    }

    public void UpdateBuildingInterior() {
        if (BuildingInterior == null) AddBuildingInterior(); //failsafe

        Transform interiorTransform = BuildingInteriorScene.GetRootGameObjects()[0].transform.GetChild(0);
        foreach (Transform buildingGameObject in interiorTransform) {
            Building building = buildingGameObject.GetComponent<Building>();
            if (building.CurrentBuildingState == Building.BuildingState.PLACED) building.DeleteBuilding();
        }

        BuildingInterior.GetComponent<Tilemap>().ClearAllTiles();
        interriorSprite = Resources.Load<Sprite>($"BuildingInsides/{InteractableBuildingComponent.GetBuildingInsideSpriteName()}");
        InteriorAreaCoordinates = GetAreaAroundPosition(Vector3Int.zero, (int)interriorSprite.textureRect.height / 16, (int)interriorSprite.textureRect.width / 16).ToArray();
        BuildingInterior.GetComponent<Tilemap>().SetTiles(InteriorAreaCoordinates, SplitSprite(interriorSprite));
        BuildingInterior.GetComponent<Tilemap>().CompressBounds();
        GetCamera().GetComponent<CameraController>().UpdateTilemapBounds();

        string BuildingName = GetComponent<InteractableBuildingComponent>().GetBuildingInsideSpriteName();
        InteriorUnavailableCoordinates = GetInsideUnavailableCoordinates(BuildingName).Select(coordinate => coordinate + InteriorAreaCoordinates[0]).ToHashSet();
        InteriorPlantableCoordinates = GetInsidePlantableCoordinates(BuildingName).Select(coordinate => coordinate + InteriorAreaCoordinates[0]).ToHashSet();
        GetMapController().UpdateAllCoordinates();
    }

    public void ToggleEditBuildingInterior() {
        if (BuildingInterior == null) AddBuildingInterior(); //failsafe

        if (interriorSprite.name != InteractableBuildingComponent.GetBuildingInsideSpriteName()) { //In case interior need to be updates
            UpdateBuildingInterior();
        }

        if (BuildingController.isInsideBuilding.Key) ExitBuildingInteriorEditing();
        else EditBuildingInterior();
    }

    public void EditBuildingInterior() {
        BuildingController.isInsideBuilding = new KeyValuePair<bool, EnterableBuildingComponent>(true, this);



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

    public override BuildingData.ComponentData Save() { //todo add saving interior
        return null;
    }

    public override void Load(BuildingData.ComponentData data) {
        return;
    }
}
