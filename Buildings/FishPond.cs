using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class FishPond : Building {

    public FishDeco deco;
    public Fish fish;

    // public FishPond(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public FishPond() : base(){
    //     Init();
    // }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 5;
        texture = Resources.Load("Buildings/Fish Pond Top") as Texture2D;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.PLACE_FISH,
            ButtonTypes.CHANGE_FISH_POND_DECO
        };
        materialsNeeded = new Dictionary<Materials, int> {
            {Materials.Coins, 5_000},
            {Materials.Wood, 200},
            {Materials.Seaweed, 5},
            {Materials.GreenAlgae, 5}
        };
    }

    /// <summary>
    ///Set the fish image and the pond color to a fish of your choosing
    /// </summary>
    /// <param name="fishType"> The fish</param>
    
    public void SetFishImage(Fish fishType){
        // GameObject fishButton = gameObject.transform.GetChild(0).gameObject;
        //if (fishButton.GetComponent<Image>().color.a == 0) fishButton.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        buttonParent.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Fish/"+ fishType.ToString());
        Color color = fishType switch
        { // RGB 0-255 dont work so these are the values normalized to 0-1
            Fish.LavaEel => new Color(0.7490196f, 0.1137255f, 0.1333333f, 1),
            Fish.SuperCucumber => new Color(0.4117647f, 0.3294118f, 0.7490196f, 1),
            Fish.Slimejack => new Color(0.08886068f, 0.7490196f, 0.003921576f, 1),
            Fish.VoidSalmon => new Color(0.5764706f, 0.1176471f, 0.7490196f, 1),
            _ => new Color(0.2039216f, 0.5254902f, 0.7490196f, 1),
        };
        tilemap.gameObject.transform.GetChild(0).GetComponent<Tilemap>().color = color;
        fish = fishType;
    }

    public void SetFishImage(){
        SetFishImage(fish);
    }

    public void CycleFishPondDeco(Building building){
        if (building == null || !(building is FishPond)) return;
        FishPond fishPond = (FishPond) building;
        Tile[] decoTiles = fishPond.deco.GetNextDeco();
        Tilemap decoTilemap = fishPond.deco.tilemap;
        decoTilemap.SetTiles(fishPond.deco?.GetPosition(), decoTiles);
    }
}
