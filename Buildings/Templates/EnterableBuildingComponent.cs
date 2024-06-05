using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.SpriteManager;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using UnityEngine.UI;

public class EnterableBuilding {
    public GameObject BuildingInterior { get; private set; }
    public Sprite interriorSprite;
    readonly IEnterableBuilding building;
    public Vector3Int[] InteriorAreaCoordinates { get; private set; }
    private float cameraSizeBeforeLock;
    private Vector3 cameraPositionBeforeLock;
    private readonly Dictionary<String, int> entranceOffsetPerBuilding; //this is so the entrace tile of the interior can match the outside entrace

    public EnterableBuilding(IEnterableBuilding building) {
        this.building = building;
        interriorSprite = Resources.Load<Sprite>($"BuildingInsides/{building.BuildingName}");
        entranceOffsetPerBuilding = new Dictionary<String, int>(){
            {"SlimeHutch", 8},
            {"Greenhouse", 10},
            {"Shed1", 10},
            {"Shed2", 8},
        };
    }

    public void AddBuildingInterior() {
        BuildingInterior = new GameObject($"{building.BuildingName} Interior");
        int middleBuildingX = building.BaseCoordinates[0].x + building.Width / 2;
        Debug.Log(interriorSprite != null ? interriorSprite.name : "Sprite is null");
        Vector3Int interiorPosition = new(middleBuildingX - entranceOffsetPerBuilding[interriorSprite.name], building.BaseCoordinates[0].y, 0);
        InteriorAreaCoordinates = GetAreaAroundPosition(interiorPosition, (int)interriorSprite.textureRect.height / 16, (int)interriorSprite.textureRect.width / 16).ToArray();
        BuildingInterior.AddComponent<Tilemap>().SetTiles(InteriorAreaCoordinates, SplitSprite(interriorSprite));
        BuildingInterior.GetComponent<Tilemap>().CompressBounds();
        BuildingInterior.AddComponent<TilemapRenderer>().sortingOrder = building.TilemapRenderer.sortingOrder + 50;
        BuildingInterior.transform.SetParent(building.Transform);
        BuildingInterior.SetActive(false);
        GameObject enterButton = building.ButtonParentGameObject.transform.Find("ENTER").gameObject;
        GameObject editButton = enterButton.transform.GetChild(0).gameObject;
        editButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "EXIT";
        if (building is IEnterableBuilding enterableBuilding) enterableBuilding.CreateInteriorCoordinates();
    }

    public void ToggleEditBuildingInterior() {
        if (BuildingInterior == null) AddBuildingInterior();
        if (BuildingController.isInsideBuilding.Key) ExitBuildingInteriorEditing();
        else EditBuildingInterior();
    }

    public void EditBuildingInterior() {
        BuildingInterior.SetActive(true);
        building.Transform.parent.GetChild(0).gameObject.SetActive(false);//map background
        BuildingController.isInsideBuilding = new KeyValuePair<bool, Transform>(true, BuildingInterior.transform);
        BuildingController.LastBuildingObjectCreated.transform.SetParent(BuildingInterior.transform);
        for (int i = 3; i < building.Transform.parent.childCount; i++) {
            if (building.Transform.parent.GetChild(i).gameObject == building.Transform.gameObject) continue;
            building.Transform.parent.GetChild(i).gameObject.SetActive(false); //disable all other buildings
        }
        cameraPositionBeforeLock = GetCamera().transform.position;
        cameraSizeBeforeLock = GetCamera().GetComponent<Camera>().orthographicSize;
        GetCamera().GetComponent<CameraController>().ToggleCameraLock();
        GetCamera().GetComponent<CameraController>().SetPosition(new Vector3(InteriorAreaCoordinates[0].x + interriorSprite.rect.width / 32, InteriorAreaCoordinates[0].y + interriorSprite.rect.height / 32, 0));
        int buildingInsideHeight = InteriorAreaCoordinates[^1].y - InteriorAreaCoordinates[0].y;
        GetCamera().GetComponent<CameraController>().SetSize(buildingInsideHeight);
        if (building is IEnterableBuilding enterableBuilding) enterableBuilding.CreateInteriorCoordinates();
        for (int i = 0; i < building.BuildingInteractions.Length; i++) {
            if (building.BuildingInteractions[i] == ButtonTypes.ENTER) continue;
            building.ButtonParentGameObject.transform.GetChild(i).gameObject.SetActive(false); //disable all other buttons
        }
        GameObject enterButton = building.ButtonParentGameObject.transform.Find("ENTER").gameObject;
        GameObject editButton = enterButton.transform.GetChild(0).gameObject;
        enterButton.transform.position = new Vector3(-400, -400, 0);
        editButton.transform.position = new Vector3(Screen.width - editButton.GetComponent<RectTransform>().rect.width / 2 - 50, editButton.GetComponent<RectTransform>().rect.height / 2 + 50, 0);
        BuildingInterior.GetComponent<TilemapRenderer>().sortingOrder = 0;
        building.TilemapRenderer.sortingOrder = -1;
        Debug.Log(editButton.activeInHierarchy);
        editButton.SetActive(true);
        Debug.Log(editButton.activeInHierarchy);
    }

    public void ExitBuildingInteriorEditing() {
        BuildingInterior.SetActive(false);
        building.Transform.parent.GetChild(0).gameObject.SetActive(true);//map background
        BuildingController.isInsideBuilding = new KeyValuePair<bool, Transform>(false, null);
        BuildingController.LastBuildingObjectCreated.transform.SetParent(building.Transform.parent);
        for (int i = 3; i < building.Transform.parent.childCount; i++) {
            if (building.Transform.parent.GetChild(i).gameObject == building.Transform.gameObject) continue; //reenable buildings
            building.Transform.parent.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = 0; i < building.BuildingInteractions.Length; i++) {
            if (building.BuildingInteractions[i] == ButtonTypes.ENTER) continue;
            building.ButtonParentGameObject.transform.GetChild(i).gameObject.SetActive(false); //reenable buttons
        }
        GetCamera().GetComponent<CameraController>().ToggleCameraLock();
        GetCamera().GetComponent<CameraController>().SetPosition(cameraPositionBeforeLock);
        GetCamera().GetComponent<CameraController>().SetSize(cameraSizeBeforeLock);
        GameObject enterButton = building.ButtonParentGameObject.transform.Find("ENTER").gameObject;
        GameObject editButton = enterButton.transform.GetChild(0).gameObject;
        editButton.SetActive(false);
        BuildingInterior.GetComponent<TilemapRenderer>().sortingOrder = building.TilemapRenderer.sortingOrder + 1;

    }
}
