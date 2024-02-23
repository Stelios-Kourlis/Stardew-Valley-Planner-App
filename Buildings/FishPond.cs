using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using System.Linq;


public class FishPond : Building {
    private SpriteAtlas atlas;
    private SpriteAtlas fishAtlas;
    public Fish fish;
    private Vector3Int[] decoCoordinates; 
    private GameObject decoTilemapObject;
    private int decoIndex = 0;
    private GameObject waterTilemapObject;

    public new void Start(){
        Init();
        base.Start();
        PlaceBuilding = Place;
        PickupBuilding = Pickup;
        atlas = Resources.Load<SpriteAtlas>("Buildings/FishPondAtlas");
        fishAtlas = Resources.Load<SpriteAtlas>("Fish/FishAtlas");
        decoTilemapObject = CreateTilemapObject(transform, 0, "Deco");
        waterTilemapObject = CreateTilemapObject(transform, 0, "Water");
        //SetFishImage(Fish.Tuna);
    }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 5;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.PLACE_FISH,
            ButtonTypes.CHANGE_FISH_POND_DECO
        };
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return new Dictionary<Materials, int> {
            {Materials.Coins, 5_000},
            {Materials.Wood, 200},
            {Materials.Seaweed, 5},
            {Materials.GreenAlgae, 5}
        };
    }

    private new void Place(Vector3Int position){
        base.Place(position);
        Vector3Int topRightCorner = baseCoordinates[0] + new Vector3Int(0, 4, 0);
        decoCoordinates = GetAreaAroundPosition(topRightCorner, 3, 5).ToArray();
        decoTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(atlas.GetSprite($"FishDeco_{decoIndex}")));
        decoTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
        waterTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        waterTilemapObject.GetComponent<Tilemap>().SetTiles(baseCoordinates, SplitSprite(atlas.GetSprite("FishPondBottom")));
        waterTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder - 1;
    }

    private new void Pickup(){
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if(!baseCoordinates.Contains(currentCell)) return;
        base.Pickup();
        decoTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        waterTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
    }

    /// <summary>
    ///Set the fish image and the pond color to a fish of your choosing
    /// </summary>
    /// <param name="fishType"> The fish</param>
    public void SetFishImage(Fish fishType){//todo readd the fish type to the button
        buttonParent.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = fishAtlas.GetSprite(fishType.ToString());
        Debug.Log(buttonParent.transform.GetChild(0).GetChild(0).name);
        Debug.Log(fishType);
        Debug.Log(fishAtlas.GetSprite(fishType.ToString()));
        Color color = fishType switch
        { // RGB 0-255 dont work so these are the values normalized to 0-1
            Fish.LavaEel => new Color(0.7490196f, 0.1137255f, 0.1333333f, 1),
            Fish.SuperCucumber => new Color(0.4117647f, 0.3294118f, 0.7490196f, 1),
            Fish.Slimejack => new Color(0.08886068f, 0.7490196f, 0.003921576f, 1),
            Fish.VoidSalmon => new Color(0.5764706f, 0.1176471f, 0.7490196f, 1),
            _ => new Color(0.2039216f, 0.5254902f, 0.7490196f, 1),
        };
        waterTilemapObject.GetComponent<Tilemap>().color = color;
        fish = fishType;
    }

    public void UpdateFishImage(){
        SetFishImage(fish);
    }

    public void CycleFishPondDeco(){
        decoIndex++;
        if (decoIndex > 3) decoIndex = 0;
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(atlas.GetSprite($"FishDeco_{decoIndex}")));
    }
}
