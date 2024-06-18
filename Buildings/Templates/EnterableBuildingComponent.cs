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
    private static readonly Dictionary<string, int> entranceOffsetPerBuilding = new(){
            {"SlimeHutch", 8},
            {"Greenhouse", 10},
            {"Shed1", 10},
            {"Shed2", 8},
        }; //this is so the entrace tile of the interior can match the outside entrace
    private InteractableBuildingComponent InteractableBuildingComponent => gameObject.GetComponent<InteractableBuildingComponent>();

    public void Awake() {
        interriorSprite = Resources.Load<Sprite>($"BuildingInsides/{Building.BuildingName}");
    }

    public void AddBuildingInterior() {
        BuildingInterior = new GameObject($"{Building.BuildingName} Interior");
        int middleBuildingX = Building.BaseCoordinates[0].x + Building.Width / 2;
        Debug.Log(interriorSprite != null ? interriorSprite.name : "Sprite is null");
        Vector3Int interiorPosition = new(middleBuildingX - entranceOffsetPerBuilding[interriorSprite.name], Building.BaseCoordinates[0].y, 0);
        InteriorAreaCoordinates = GetAreaAroundPosition(interiorPosition, (int)interriorSprite.textureRect.height / 16, (int)interriorSprite.textureRect.width / 16).ToArray();
        BuildingInterior.AddComponent<Tilemap>().SetTiles(InteriorAreaCoordinates, SplitSprite(interriorSprite));
        BuildingInterior.GetComponent<Tilemap>().CompressBounds();
        BuildingInterior.AddComponent<TilemapRenderer>().sortingOrder = Building.TilemapRenderer.sortingOrder + 50;
        BuildingInterior.transform.SetParent(Building.Transform);
        BuildingInterior.SetActive(false);
        GameObject enterButton = InteractableBuildingComponent.ButtonParentGameObject.transform.Find("ENTER").gameObject;
        GameObject editButton = enterButton.transform.GetChild(0).gameObject;
        editButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "EXIT";
        if (Building is IEnterableBuilding enterableBuilding) enterableBuilding.CreateInteriorCoordinates();
    }

    public void ToggleEditBuildingInterior() {
        if (BuildingInterior == null) AddBuildingInterior();
        if (BuildingController.isInsideBuilding.Key) ExitBuildingInteriorEditing();
        else EditBuildingInterior();
    }

    public void EditBuildingInterior() {
        BuildingInterior.SetActive(true);
        Building.Transform.parent.GetChild(0).gameObject.SetActive(false);//map background
        BuildingController.isInsideBuilding = new KeyValuePair<bool, Transform>(true, BuildingInterior.transform);
        BuildingController.LastBuildingObjectCreated.transform.SetParent(BuildingInterior.transform);
        for (int i = 3; i < Building.Transform.parent.childCount; i++) {
            if (Building.Transform.parent.GetChild(i).gameObject == Building.Transform.gameObject) continue;
            Building.Transform.parent.GetChild(i).gameObject.SetActive(false); //disable all other buildings
        }
        cameraPositionBeforeLock = GetCamera().transform.position;
        cameraSizeBeforeLock = GetCamera().GetComponent<Camera>().orthographicSize;
        GetCamera().GetComponent<CameraController>().ToggleCameraLock();
        GetCamera().GetComponent<CameraController>().SetPosition(new Vector3(InteriorAreaCoordinates[0].x + interriorSprite.rect.width / 32, InteriorAreaCoordinates[0].y + interriorSprite.rect.height / 32, 0));
        int buildingInsideHeight = InteriorAreaCoordinates[^1].y - InteriorAreaCoordinates[0].y;
        GetCamera().GetComponent<CameraController>().SetSize(buildingInsideHeight);
        if (Building is IEnterableBuilding enterableBuilding) enterableBuilding.CreateInteriorCoordinates();
        for (int i = 0; i < InteractableBuildingComponent.BuildingInteractions.Length; i++) {
            if (InteractableBuildingComponent.BuildingInteractions[i] == ButtonTypes.ENTER) continue;
            InteractableBuildingComponent.ButtonParentGameObject.transform.GetChild(i).gameObject.SetActive(false); //disable all other buttons
        }
        GameObject enterButton = InteractableBuildingComponent.ButtonParentGameObject.transform.Find("ENTER").gameObject;
        GameObject editButton = enterButton.transform.GetChild(0).gameObject;
        enterButton.transform.position = new Vector3(-400, -400, 0);
        editButton.transform.position = new Vector3(Screen.width - editButton.GetComponent<RectTransform>().rect.width / 2 - 50, editButton.GetComponent<RectTransform>().rect.height / 2 + 50, 0);
        BuildingInterior.GetComponent<TilemapRenderer>().sortingOrder = 0;
        Building.TilemapRenderer.sortingOrder = -1;
        Debug.Log(editButton.activeInHierarchy);
        editButton.SetActive(true);
        Debug.Log(editButton.activeInHierarchy);
    }

    public void ExitBuildingInteriorEditing() {
        BuildingInterior.SetActive(false);
        Building.Transform.parent.GetChild(0).gameObject.SetActive(true);//map background
        BuildingController.isInsideBuilding = new KeyValuePair<bool, Transform>(false, null);
        BuildingController.LastBuildingObjectCreated.transform.SetParent(Building.Transform.parent);
        for (int i = 3; i < Building.Transform.parent.childCount; i++) {
            if (Building.Transform.parent.GetChild(i).gameObject == Building.Transform.gameObject) continue; //reenable buildings
            Building.Transform.parent.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < InteractableBuildingComponent.BuildingInteractions.Length; i++) {
            if (InteractableBuildingComponent.BuildingInteractions[i] == ButtonTypes.ENTER) continue;
            InteractableBuildingComponent.ButtonParentGameObject.transform.GetChild(i).gameObject.SetActive(false); //reenable buttons
        }
        GetCamera().GetComponent<CameraController>().ToggleCameraLock();
        GetCamera().GetComponent<CameraController>().SetPosition(cameraPositionBeforeLock);
        GetCamera().GetComponent<CameraController>().SetSize(cameraSizeBeforeLock);
        GameObject enterButton = InteractableBuildingComponent.ButtonParentGameObject.transform.Find("ENTER").gameObject;
        GameObject editButton = enterButton.transform.GetChild(0).gameObject;
        editButton.SetActive(false);
        BuildingInterior.GetComponent<TilemapRenderer>().sortingOrder = Building.TilemapRenderer.sortingOrder + 1;

    }
}
