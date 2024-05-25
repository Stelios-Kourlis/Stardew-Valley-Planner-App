using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.SpriteManager;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using UnityEngine.UI;

public class EnterableBuilding{
    public GameObject BuildingInterior {get; private set;}
    public Sprite interriorSprite;
    readonly Building building;
    public Vector3Int[] InteriorAreaCoordinates {get; private set;}
    private float cameraSizeBeforeLock;
    private Vector3 cameraPositionBeforeLock;
    private readonly Dictionary<Type, int> entranceOffsetPerBuilding; //this is so the entrace tile of the interior can match the outside entrace

    public EnterableBuilding(Building building){
        this.building = building;
        interriorSprite = Resources.Load<Sprite>($"BuildingInsides/{building.name}");
        entranceOffsetPerBuilding = new Dictionary<Type, int>(){
            {typeof(SlimeHutch), 8},
            {typeof(Greenhouse), 10}
        };
    }

    public void AddBuildingInterior(){
        BuildingInterior = new GameObject($"{building.name} Interior");
        int middleBuildingX = building.BaseCoordinates[0].x + building.Width / 2;
        Vector3Int interiorPosition = new(middleBuildingX - entranceOffsetPerBuilding[building.GetType()], building.BaseCoordinates[0].y, 0);
        InteriorAreaCoordinates = GetAreaAroundPosition(interiorPosition, (int)interriorSprite.textureRect.height / 16, (int)interriorSprite.textureRect.width / 16).ToArray();
        BuildingInterior.AddComponent<Tilemap>().SetTiles(InteriorAreaCoordinates, SplitSprite(interriorSprite));
        BuildingInterior.AddComponent<TilemapRenderer>().sortingOrder = building.gameObject.GetComponent<TilemapRenderer>().sortingOrder + 50;
        BuildingInterior.transform.SetParent(building.transform);
        BuildingInterior.SetActive(false);
        GameObject enterButton = building.buttonParent.transform.Find("ENTER").gameObject;
        GameObject editButton = enterButton.transform.GetChild(0).gameObject;
        editButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "EXIT";
    }

    public void ToggleEditBuildingInterior(){
        if (GetBuildingController().isInsideBuilding.Key) ExitBuildingInteriorEditing();
        else EditBuildingInterior();
    }

    public void EditBuildingInterior(){
        BuildingInterior.SetActive(true);
        building.transform.parent.GetChild(0).gameObject.SetActive(false);//map background
        GetBuildingController().isInsideBuilding = new KeyValuePair<bool, Transform>(true, BuildingInterior.transform);
        GetBuildingController().lastBuildingObjectCreated.transform.SetParent(BuildingInterior.transform);
        for (int i = 3; i < building.transform.parent.childCount; i++){
            if (building.transform.parent.GetChild(i).gameObject == building.gameObject) continue;
            building.transform.parent.GetChild(i).gameObject.SetActive(false);
        }
        cameraPositionBeforeLock = GetCamera().transform.position;
        cameraSizeBeforeLock = GetCamera().GetComponent<Camera>().orthographicSize;
        GetCamera().GetComponent<CameraController>().ToggleCameraLock();
        GetCamera().GetComponent<CameraController>().SetPosition(new Vector3(InteriorAreaCoordinates[0].x + interriorSprite.rect.width / 32, InteriorAreaCoordinates[0].y + interriorSprite.rect.height / 32 ,0));
        GetCamera().GetComponent<CameraController>().SetSize(10);
        GameObject enterButton = building.buttonParent.transform.Find("ENTER").gameObject;
        GameObject editButton = enterButton.transform.GetChild(0).gameObject;
        enterButton.transform.position = new Vector3(-400, -400, 0);
        editButton.transform.position = new Vector3(Screen.width - editButton.GetComponent<RectTransform>().rect.width / 2 - 50 , editButton.GetComponent<RectTransform>().rect.height / 2 + 50, 0);
        BuildingInterior.GetComponent<TilemapRenderer>().sortingOrder = 0;
        building.gameObject.GetComponent<TilemapRenderer>().sortingOrder = -1;
        Debug.Log(editButton.activeInHierarchy);
        editButton.SetActive(true);
        Debug.Log(editButton.activeInHierarchy);
    }

    public void ExitBuildingInteriorEditing(){
        BuildingInterior.SetActive(false);
        building.transform.parent.GetChild(0).gameObject.SetActive(true);//map background
        GetBuildingController().isInsideBuilding = new KeyValuePair<bool, Transform>(false, null);
        GetBuildingController().lastBuildingObjectCreated.transform.SetParent(building.transform.parent);
        for (int i = 3; i < building.transform.parent.childCount; i++){
            if (building.transform.parent.GetChild(i).gameObject == building.gameObject) continue;
            building.transform.parent.GetChild(i).gameObject.SetActive(true);
        }
        GetCamera().GetComponent<CameraController>().ToggleCameraLock();
        GetCamera().GetComponent<CameraController>().SetPosition(cameraPositionBeforeLock);
        GetCamera().GetComponent<CameraController>().SetSize(cameraSizeBeforeLock);
        GameObject enterButton = building.buttonParent.transform.Find("ENTER").gameObject;
        GameObject editButton = enterButton.transform.GetChild(0).gameObject;
        editButton.SetActive(false);
        BuildingInterior.GetComponent<TilemapRenderer>().sortingOrder = building.gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
        
    }
}
