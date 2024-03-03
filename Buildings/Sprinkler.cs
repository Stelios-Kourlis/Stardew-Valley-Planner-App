using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using static Utility.SpriteManager;

public class Sprinkler : Building, ITieredBuilding{

    private SpriteAtlas atlas;
    public int Tier {get; private set;}
    private Tile greenTile;

    public new void Start(){
        name = GetType().Name;
        baseHeight = 1;
        base.Start();
        atlas = Resources.Load<SpriteAtlas>("Buildings/SprinklerAtlas");
        greenTile = LoadTile("GreenTile");
        ChangeTier(1);
    }

    public void ChangeTier(int tier){
        if (tier < 0 || tier > 3) throw new System.ArgumentException($"Tier must be between 1 and 3 (got {tier})");
        Tier = tier;
        UpdateTexture(atlas.GetSprite($"SprinklerT{tier}"));
    }

    protected override void PlacePreview(){
        if (hasBeenPlaced) return;
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        base.PlacePreview();
        Vector3Int[] coverageArea = Tier switch{
            1 => GetCrossAroundPosition(currentCell).ToArray(),
            2 => GetAreaAroundPosition(currentCell, 1).ToArray(),
            3 => GetAreaAroundPosition(currentCell, 2).ToArray(),
            _ => throw new System.ArgumentException($"Invalid tier {Tier}")
        };
        foreach (Vector3Int cell in coverageArea) GetComponent<Tilemap>().SetTile(cell, greenTile);
    }

    public override void Place(Vector3Int position){
        base.Place(position);
        Vector3Int[] coverageArea = Tier switch{
            1 => GetCrossAroundPosition(position).ToArray(),
            2 => GetAreaAroundPosition(position, 1).ToArray(),
            3 => GetAreaAroundPosition(position, 2).ToArray(),
            _ => throw new System.ArgumentException($"Invalid tier {Tier}")
        };
        foreach (Vector3Int cell in coverageArea) GetComponent<Tilemap>().SetTile(cell, null);
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return Tier switch{
            1 => new List<MaterialInfo>{
                new MaterialInfo(1, Materials.IronBar),
                new MaterialInfo(1, Materials.CopperBar),
            },
            2 => new List<MaterialInfo>{
                new MaterialInfo(1, Materials.GoldBar),
                new MaterialInfo(1, Materials.IronBar),
                new MaterialInfo(1, Materials.RefinedQuartz)
            },
            3 => new List<MaterialInfo>{
                new MaterialInfo(1, Materials.IridiumBar),
                new MaterialInfo(1, Materials.GoldBar),
                new MaterialInfo(1, Materials.BatteryPack),
            },
            _ => throw new System.ArgumentException($"Invalid tier {Tier}")
        };
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{Tier}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Start();
        Place(new Vector3Int(x,y,0));
        ChangeTier(int.Parse(data[0]));
    }

    
}
