using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.SpriteManager;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class EnterableBuildingComponent : MonoBehaviour {
    public GameObject BuildingInterior { get; private set; }
    public Sprite interriorSprite;
    public Building Building => gameObject.GetComponent<Building>();
    public Vector3Int[] InteriorAreaCoordinates { get; private set; }
    public HashSet<Vector3Int> InteriorUnavailableCoordinates { get; private set; }
    public HashSet<Vector3Int> InteriorPlantableCoordinates { get; private set; }
    public static Action EnteredOrExitedBuilding { get; set; }
    private static int numberOfInteriors = 0;
    private Vector3 cameraPositionBeforeEnter;
    Scene BuildingInteriorScene;
    private static readonly Dictionary<string, int> entranceOffsetPerBuilding = new(){
            {"SlimeHutch", 8},
            {"Greenhouse", 10},
            {"Shed1", 10},
            {"Shed2", 8},
            {"Barn1", 11},
            {"Barn2", 11},
            {"Barn3", 11},
            {"House1", 3},
            {"House2", 10},
            {"House3", 13},
            {"Cabin1", 3},
            {"Cabin2", 10},
            {"Cabin3", 13}
        }; //this is so the entrace tile of the interior can match the outside entrace
    private InteractableBuildingComponent InteractableBuildingComponent => gameObject.GetComponent<InteractableBuildingComponent>();
    private Scene AppScene => SceneManager.GetSceneByName("App");

    public void Awake() {
        Building.BuildingPlaced += AddBuildingInterior;
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.ENTER);

    }

    public void AddBuildingInterior() {
        BuildingInteriorScene = SceneManager.CreateScene($"BuildingInterior{numberOfInteriors++} ({Building.BuildingName})");
        interriorSprite = Resources.Load<Sprite>($"BuildingInsides/{InteractableBuildingComponent.GetBuildingInsideSpriteName()}");
        BuildingInterior = new GameObject($"{Building.BuildingName} Interior");
        InteriorAreaCoordinates = GetAreaAroundPosition(Vector3Int.zero, (int)interriorSprite.textureRect.height / 16, (int)interriorSprite.textureRect.width / 16).ToArray();
        BuildingInterior.AddComponent<Tilemap>().SetTiles(InteriorAreaCoordinates, SplitSprite(interriorSprite));
        BuildingInterior.GetComponent<Tilemap>().CompressBounds();
        BuildingInterior.AddComponent<TilemapRenderer>().sortingOrder = Building.TilemapRenderer.sortingOrder + 50;

        GameObject grid = new("Grid");
        grid.SetActive(false);
        grid.AddComponent<Grid>();
        grid.AddComponent<Tilemap>();
        BuildingInterior.transform.SetParent(grid.transform);

        SceneManager.MoveGameObjectToScene(grid, BuildingInteriorScene);

        // string BuildingName = Building.GetType().ToString();
        // if (BuildingName.Contains("Cabin")) BuildingName = "House";
        // BuildingName += (Building is ITieredBuilding tieredBuilding) ? tieredBuilding.Tier : "";
        // InteriorUnavailableCoordinates = BuildingUnavailableCoordinatesController.GetInsideUnavailableCoordinates(BuildingName).Select(coordinate => coordinate + InteriorAreaCoordinates[0]).ToHashSet();
        // InteriorPlantableCoordinates = BuildingUnavailableCoordinatesController.GetInsidePlantableCoordinates(BuildingName).Select(coordinate => coordinate + InteriorAreaCoordinates[0]).ToHashSet();
    }

    public void ToggleEditBuildingInterior() {
        if (BuildingInterior == null) AddBuildingInterior(); //failsafe

        if (interriorSprite.name != InteractableBuildingComponent.GetBuildingInsideSpriteName()) { //In case interior need to be updates
            Destroy(BuildingInterior);
            AddBuildingInterior();
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
        GetCamera().GetComponent<CameraController>().SetPosition(new Vector3(interriorSprite.textureRect.width / 16 / 2, interriorSprite.textureRect.height / 16 / 2, 0)); //center the camera on the interior
        SceneManager.MoveGameObjectToScene(GetCamera(), BuildingInteriorScene);

        foreach (ButtonTypes type in InteractableBuildingComponent.BuildingInteractions) {
            if (type == ButtonTypes.ENTER) continue;
            InteractableBuildingComponent.ButtonParentGameObject.transform.Find(type.ToString()).gameObject.SetActive(false);
        }

        for (int i = 0; i < Building.Transform.parent.childCount; i++) {
            if (Building.Transform.parent.GetChild(i).gameObject == Building.Transform.gameObject) Building.Transform.gameObject.GetComponent<Tilemap>().color = new Color(1, 1, 1, 0); //make building transparent
            else Building.Transform.parent.GetChild(i).gameObject.SetActive(false); //disable all other buildings
        }

        foreach (GameObject obj in AppScene.GetRootGameObjects()) {
            if (obj.name == "Canvas") continue;
            if (obj.name == "EventSystem") continue;
            if (obj.name == "Logic") continue;
            if (obj.name == "Grid") continue;
            obj.SetActive(false);
        }

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
        SceneManager.MoveGameObjectToScene(GetCamera(), AppScene);

        foreach (ButtonTypes type in InteractableBuildingComponent.BuildingInteractions) {
            if (type == ButtonTypes.ENTER) continue;
            InteractableBuildingComponent.ButtonParentGameObject.transform.Find(type.ToString()).gameObject.SetActive(true);
        }

        for (int i = 0; i < Building.Transform.parent.childCount; i++) {
            if (Building.Transform.parent.GetChild(i).gameObject == Building.Transform.gameObject) Building.Transform.gameObject.GetComponent<Tilemap>().color = new Color(1, 1, 1, 1); //make building transparent
            else Building.Transform.parent.GetChild(i).gameObject.SetActive(true); //disable all other buildings
        }

        foreach (GameObject obj in AppScene.GetRootGameObjects()) {
            obj.SetActive(true);
        }

        SceneManager.SetActiveScene(AppScene);

        EnteredOrExitedBuilding?.Invoke();
        BuildingController.LastBuildingObjectCreated.transform.SetParent(GetGridTilemap().transform);

    }
}