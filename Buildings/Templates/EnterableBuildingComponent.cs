using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.SpriteManager;
using static Utility.TilemapManager;

public class EnterableBuildingComponent{
    public GameObject BuildingInterior {get; private set;}
    Sprite interriorSprite;
    readonly Building building;
    private bool isActive;

    public EnterableBuildingComponent(Building building){
        this.building = building;
        interriorSprite = Resources.Load<Sprite>($"BuildingInsides/{building.name}");
    }

    public void AddBuildingInterior(){
        BuildingInterior = new GameObject($"{building.name} Interior");
        Vector3Int[] interiorArea = GetAreaAroundPosition(building.BaseCoordinates[0], (int)interriorSprite.textureRect.height / 16, (int)interriorSprite.textureRect.width / 16).ToArray();
        BuildingInterior.AddComponent<Tilemap>().SetTiles(interiorArea, SplitSprite(interriorSprite));
        BuildingInterior.AddComponent<TilemapRenderer>().sortingOrder = building.gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
        BuildingInterior.transform.SetParent(building.transform);
        HideBuildingInterior();
    }

    public void ToggleBuildingInterior(){
        if (isActive) HideBuildingInterior();
        else ShowBuildingInterior();
    }

    public void ShowBuildingInterior(){
        BuildingInterior.SetActive(true);
        isActive = true;
    }

    public void EditBuildingInterior(){

    }

    public void HideBuildingInterior(){
        BuildingInterior.SetActive(false);
        isActive = false;
    }
}
