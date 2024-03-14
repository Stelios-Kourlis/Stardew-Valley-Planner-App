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
    public override string TooltipMessage => "Right Click For More Options";

    public override void OnAwake(){
        name = GetType().Name;
        baseHeight = 5;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.PLACE_FISH,
            ButtonTypes.CHANGE_FISH_POND_DECO
        };
        base.OnAwake();
        atlas = Resources.Load<SpriteAtlas>("Buildings/FishPondAtlas");
        fishAtlas = Resources.Load<SpriteAtlas>("Fish/FishAtlas");
        decoTilemapObject = CreateTilemapObject(transform, 0, "Deco");
        waterTilemapObject = CreateTilemapObject(transform, 0, "Water");
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo> {
            new MaterialInfo(5_000, Materials.Coins),
            new MaterialInfo(200, Materials.Wood),
            new MaterialInfo(5, Materials.Seaweed),
            new MaterialInfo(5, Materials.GreenAlgae)
        };
    }

    public override void Place(Vector3Int position){
        base.Place(position);
        Vector3Int topRightCorner = position + new Vector3Int(0, 4, 0);
        decoCoordinates = GetAreaAroundPosition(topRightCorner, 3, 5).ToArray();
        decoTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        decoTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(atlas.GetSprite($"FishDeco_{decoIndex}")));
        decoTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
        waterTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        waterTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
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

    protected override void PlacePreview(Vector3Int position){
        if (hasBeenPlaced) return;
        base.PlacePreview(position);
        Vector3Int topRightCorner = position + new Vector3Int(0, 4, 0);
        decoCoordinates = GetAreaAroundPosition(topRightCorner, 3, 5).ToArray();
        decoTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        Vector3Int[] unavailableCoordinates = GetBuildingController().GetUnavailableCoordinates().ToArray();
        Vector3Int[] buildingBaseCoordinates = GetAreaAroundPosition(position, baseHeight, width).ToArray();
        if (unavailableCoordinates.Intersect(buildingBaseCoordinates).Count() != 0) decoTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else decoTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(atlas.GetSprite($"FishDeco_{decoIndex}")));
        decoTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
        waterTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        if (unavailableCoordinates.Intersect(buildingBaseCoordinates).Count() != 0) waterTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else waterTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        waterTilemapObject.GetComponent<Tilemap>().SetTiles(GetAreaAroundPosition(position, height, width).ToArray(), SplitSprite(atlas.GetSprite("FishPondBottom")));
        waterTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder - 1;
    }

    protected override void PickupPreview(){
        if (!hasBeenPlaced) return;
        base.PickupPreview();
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (baseCoordinates.Contains(currentCell)){
            decoTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
            waterTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        }
        else{
            decoTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
            waterTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        }
    }

    protected override void DeletePreview(){
        if (!hasBeenPlaced) return;
        base.DeletePreview();
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (baseCoordinates.Contains(currentCell)){
            decoTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
            waterTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        }
        else{
            decoTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
            waterTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        }
    }

    /// <summary>
    ///Set the fish image and the pond color to a fish of your choosing
    /// </summary>
    /// <param name="fishType"> The fish</param>
    public void SetFishImage(Fish fishType){
        buttonParent.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = fishAtlas.GetSprite(fishType.ToString());
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

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{decoIndex}|{(int)fish}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
        SetFishImage((Fish) int.Parse(data[1]) );
        decoTilemapObject.GetComponent<Tilemap>().SetTiles(decoCoordinates, SplitSprite(atlas.GetSprite($"FishDeco_{data[0]}")));
    }
}
