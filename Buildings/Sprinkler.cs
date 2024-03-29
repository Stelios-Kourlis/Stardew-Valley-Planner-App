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
    public override string TooltipMessage => "Right Click For More Options";

    private SpriteAtlas atlas;
    public int Tier {get; private set;}
    private Tile greenTile;
    private bool HasPressureNozzle {get; set;} = false;
    private bool HasEnricher {get; set;} = false;

    public override void OnAwake(){
        name = GetType().Name;
        baseHeight = 1;
        base.OnAwake();
        atlas = Resources.Load<SpriteAtlas>("Buildings/SprinklerAtlas");
        greenTile = LoadTile("GreenTile");
        ChangeTier(1);
    }

    public void ChangeTier(int tier){
        if (tier < 0 || tier > 3) throw new System.ArgumentException($"Tier must be between 1 and 3 (got {tier})");
        Tier = tier;
        UpdateTexture(atlas.GetSprite($"Sprinkler{tier}"));
    }

    protected override void PlacePreview(Vector3Int position){
        if (hasBeenPlaced) return;
        base.PlacePreview(position);
        Vector3Int[] coverageArea = Tier switch{
            1 => GetCrossAroundPosition(position).ToArray(),
            2 => GetAreaAroundPosition(position, 1).ToArray(),
            3 => GetAreaAroundPosition(position, 2).ToArray(),
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
        OnAwake();
        Place(new Vector3Int(x,y,0));
        ChangeTier(int.Parse(data[0]));
    }

    protected override void OnMouseRightClick(){
        if (HasPressureNozzle){
            HasEnricher = true;
            HasPressureNozzle = false;
            UpdateTexture(atlas.GetSprite($"Sprinkler{Tier}Enricher"));
        }else if (HasEnricher){
            HasEnricher = false;
            HasPressureNozzle = true;
            UpdateTexture(atlas.GetSprite($"Sprinkler{Tier}PressureNozzle"));
        }else{
            HasPressureNozzle = true;
        }
        UpdateTexture(atlas.GetSprite($"Sprinkler{Tier}"));
    }
}
