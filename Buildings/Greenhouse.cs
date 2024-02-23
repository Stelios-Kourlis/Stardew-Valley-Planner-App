using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;
using static Utility.SpriteManager;

public class Greenhouse : Building {

    private GameObject porchTilemapObject;
    private Sprite porchSprite;


    protected override void Init(){
        name = GetType().Name;
        baseHeight = 6;
        insideAreaTexture = Resources.Load("BuildingInsides/Greenhouse") as Texture2D;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.ENTER
        };
    }

    public new void Start(){
        Init();
        base.Start();
        PlaceBuilding = Place;
        PickupBuilding = Pickup;
        porchSprite = Resources.Load<Sprite>("Buildings/GreenhousePorch");
        porchTilemapObject = CreateTilemapObject(transform, 0, "Porch");
    }

    private new void Place(Vector3Int position){
        base.Place(position);
        Vector3Int porchBottomRight = baseCoordinates[0] + new Vector3Int(2, 0, 0) - new Vector3Int(0, 2, 0);
        Vector3Int[] porchCoordinates = GetAreaAroundPosition(porchBottomRight, 2, 3).ToArray();
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        porchTilemapObject.GetComponent<Tilemap>().SetTiles(porchCoordinates, SplitSprite(porchSprite));
        porchTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
    }

    private new void Pickup(){
        base.Pickup();
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
    }
}
