using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.SpriteManager;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using UnityEngine.UI;

public class EnterableBuildingComponent{
    public GameObject BuildingInterior {get; private set;}
    Sprite interriorSprite;
    readonly Building building;
    private bool isActive;
    public Vector3Int[] InteriorAreaCoordinates {get; private set;}
    private float cameraSizeBeforeLock;
    private Vector3 cameraPositionBeforeLock;
    private readonly Dictionary<Type, int> entranceOffsetPerBuilding; //this is so the entrace tile of the interior can match the outside entrace

    public EnterableBuildingComponent(Building building){
        this.building = building;
        interriorSprite = Resources.Load<Sprite>($"BuildingInsides/{building.name}");
        entranceOffsetPerBuilding = new Dictionary<Type, int>(){
            {typeof(SlimeHutch), 8}
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
        HideBuildingInterior();
    }

    public void ToggleBuildingInterior(){
        if (isActive) HideBuildingInterior();
        else ShowBuildingInterior();
    }

    public void HideBuildingInterior(){
        BuildingInterior.SetActive(false);
        isActive = false;
    }

    public void ShowBuildingInterior(){
        BuildingInterior.SetActive(true);
        isActive = true;
    }

    public void ToggleEditBuildingInterior(){
        if (GetBuildingController().isInsideBuilding.Key) ExitBuildingInteriorEditing();
        else EditBuildingInterior();
    }

    public void EditBuildingInterior(){
        building.transform.parent.GetChild(0).gameObject.SetActive(false);
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
        editButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "EXIT";
        BuildingInterior.GetComponent<TilemapRenderer>().sortingOrder = 0;
        building.gameObject.GetComponent<TilemapRenderer>().sortingOrder = -1;
    }

    public void ExitBuildingInteriorEditing(){
        building.transform.parent.GetChild(0).gameObject.SetActive(true);
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
        //todo depending on camera orth. size it ends up in diffrent positions
        editButton.transform.position = enterButton.transform.position + new Vector3(editButton.GetComponent<RectTransform>().rect.width,0,0);
        editButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "EDIT";
        building.gameObject.GetComponent<TilemapRenderer>().sortingOrder = building.BaseCoordinates[0].y;
        BuildingInterior.GetComponent<TilemapRenderer>().sortingOrder = building.gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
        
    }
}
