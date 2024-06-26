using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.SpriteManager;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using UnityEngine.UI;

public class EnterableBuildingComponent : MonoBehaviour {
    public GameObject BuildingInterior { get; private set; }
    public Sprite interriorSprite;
    private IEnterableBuilding Building => gameObject.GetComponent<IEnterableBuilding>();
    public Vector3Int[] InteriorAreaCoordinates { get; private set; }
    private float cameraSizeBeforeLock;
    private Vector3 cameraPositionBeforeLock;
    public static Action EnteredOrExitedBuilding { get; set; }
    private static readonly Dictionary<string, int> entranceOffsetPerBuilding = new(){
            {"SlimeHutch", 8},
            {"Greenhouse", 10},
            {"Shed1", 10},
            {"Shed2", 8},
            {"Barn1", 11},
            {"Barn2", 11},
            {"Barn3", 11},
        }; //this is so the entrace tile of the interior can match the outside entrace
    private InteractableBuildingComponent InteractableBuildingComponent => gameObject.GetComponent<InteractableBuildingComponent>();

    public void Awake() {
        Building.BuildingPlaced += AddBuildingInterior;
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.ENTER);

    }

    public void AddBuildingInterior() {
        interriorSprite = Resources.Load<Sprite>($"BuildingInsides/{InteractableBuildingComponent.GetBuildingSpriteName()}");
        if (interriorSprite == null) return;//todo when all interiors are addded, delete this since it will be impossible to have a null sprite
        BuildingInterior = new GameObject($"{Building.BuildingName} Interior");
        int middleBuildingX = Building.BaseCoordinates[0].x + Building.Width / 2;
        Vector3Int interiorPosition = new(middleBuildingX - entranceOffsetPerBuilding[interriorSprite.name], Building.BaseCoordinates[0].y, 0);
        InteriorAreaCoordinates = GetAreaAroundPosition(interiorPosition, (int)interriorSprite.textureRect.height / 16, (int)interriorSprite.textureRect.width / 16).ToArray();
        BuildingInterior.AddComponent<Tilemap>().SetTiles(InteriorAreaCoordinates, SplitSprite(interriorSprite));
        BuildingInterior.GetComponent<Tilemap>().CompressBounds();
        BuildingInterior.AddComponent<TilemapRenderer>().sortingOrder = Building.TilemapRenderer.sortingOrder + 50;
        BuildingInterior.transform.SetParent(Building.Transform);
        BuildingInterior.SetActive(false);
        if (Building is IEnterableBuilding enterableBuilding) enterableBuilding.CreateInteriorCoordinates();
    }

    public void ToggleEditBuildingInterior() {
        if (BuildingInterior == null) AddBuildingInterior(); //failsafe

        if (interriorSprite.name != InteractableBuildingComponent.GetBuildingSpriteName()) { //In case interior need to be updates
            Destroy(BuildingInterior);
            AddBuildingInterior();
        }

        if (BuildingController.isInsideBuilding.Key) ExitBuildingInteriorEditing();
        else EditBuildingInterior();
    }

    public void EditBuildingInterior() {
        BuildingInterior.SetActive(true);
        Building.Transform.parent.GetChild(0).gameObject.SetActive(false);//map background
        BuildingController.isInsideBuilding = new KeyValuePair<bool, Transform>(true, BuildingInterior.transform);
        EnteredOrExitedBuilding?.Invoke();
        BuildingController.LastBuildingObjectCreated.transform.SetParent(BuildingInterior.transform);
        for (int i = 3; i < Building.Transform.parent.childCount; i++) {
            if (Building.Transform.parent.GetChild(i).gameObject == Building.Transform.gameObject) continue;
            Building.Transform.parent.GetChild(i).gameObject.SetActive(false); //disable all other buildings
        }
        SetUpCamera();
        if (Building is IEnterableBuilding enterableBuilding) enterableBuilding.CreateInteriorCoordinates();
        for (int i = 0; i < InteractableBuildingComponent.BuildingInteractions.Count; i++) {
            if (InteractableBuildingComponent.BuildingInteractions[i] == ButtonTypes.ENTER) continue;
            InteractableBuildingComponent.ButtonParentGameObject.transform.GetChild(i).gameObject.SetActive(false); //disable all other buttons
        }
        GameObject enterButton = InteractableBuildingComponent.ButtonParentGameObject.transform.Find("ENTER").gameObject;
        enterButton.transform.position = new Vector3(Screen.width - enterButton.GetComponent<RectTransform>().rect.width / 2 - 50, enterButton.GetComponent<RectTransform>().rect.height / 2 + 50, 0);
        BuildingInterior.GetComponent<TilemapRenderer>().sortingOrder = 0;
        Building.TilemapRenderer.sortingOrder = -1;
    }

    private void SetUpCamera() {
        cameraPositionBeforeLock = GetCamera().transform.position;
        cameraSizeBeforeLock = GetCamera().GetComponent<Camera>().orthographicSize;
        GetCamera().GetComponent<CameraController>().ToggleCameraLock();
        var tilemapBounds = BuildingInterior.GetComponent<Tilemap>().localBounds;
        var camera = GetCamera().GetComponent<Camera>();
        float tilemapWidth = tilemapBounds.size.x;
        float tilemapHeight = tilemapBounds.size.y;
        Vector3 tilemapCenter = tilemapBounds.center;

        // Determine if the tilemap is wider or taller than what the camera can show
        if ((tilemapWidth / tilemapHeight) > cameraSizeBeforeLock) {
            // Tilemap is wider than the camera's aspect ratio
            // Adjust camera size based on the width
            GetCamera().GetComponent<CameraController>().SetSize(tilemapWidth / cameraSizeBeforeLock / 2);
        }
        else {
            // Tilemap is taller than the camera's aspect ratio
            // Adjust camera size based on the height
            GetCamera().GetComponent<CameraController>().SetSize(tilemapHeight / 2);
        }

        // Center the camera on the tilemap
        GetCamera().GetComponent<CameraController>().SetPosition(new Vector3(tilemapCenter.x, tilemapCenter.y, camera.transform.position.z));
        // GetCamera().GetComponent<CameraController>().SetPosition(new Vector3(InteriorAreaCoordinates[0].x + interriorSprite.rect.width / 32, InteriorAreaCoordinates[0].y + interriorSprite.rect.height / 32, 0));
    }

    public void ExitBuildingInteriorEditing() {
        BuildingInterior.SetActive(false);
        Building.Transform.parent.GetChild(0).gameObject.SetActive(true);//map background
        BuildingController.isInsideBuilding = new KeyValuePair<bool, Transform>(false, null);
        EnteredOrExitedBuilding?.Invoke();
        BuildingController.LastBuildingObjectCreated.transform.SetParent(Building.Transform.parent);
        for (int i = 3; i < Building.Transform.parent.childCount; i++) {
            if (Building.Transform.parent.GetChild(i).gameObject == Building.Transform.gameObject) continue; //reenable buildings
            Building.Transform.parent.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < InteractableBuildingComponent.BuildingInteractions.Count; i++) {
            if (InteractableBuildingComponent.BuildingInteractions[i] == ButtonTypes.ENTER) continue;
            InteractableBuildingComponent.ButtonParentGameObject.transform.GetChild(i).gameObject.SetActive(true); //reenable buttons
        }
        GetCamera().GetComponent<CameraController>().ToggleCameraLock();
        GetCamera().GetComponent<CameraController>().SetPosition(cameraPositionBeforeLock);
        GetCamera().GetComponent<CameraController>().SetSize(cameraSizeBeforeLock);
        GameObject enterButton = InteractableBuildingComponent.ButtonParentGameObject.transform.Find("ENTER").gameObject;
        BuildingInterior.GetComponent<TilemapRenderer>().sortingOrder = Building.TilemapRenderer.sortingOrder + 1;

    }
}
