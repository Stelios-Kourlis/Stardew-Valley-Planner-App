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
    private int tier;
    private Tile greenTile;

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 1;
    }

    public new void Start(){
        Init();
        base.Start();
        PlaceBuilding = Place;
        PlacePreview = PlaceMouseoverEffect;
        atlas = Resources.Load<SpriteAtlas>("Buildings/SprinklerAtlas");
        greenTile = LoadTile("GreenTile");
        ChangeTier(1);
    }

    public void ChangeTier(int tier){
        if (tier < 0 || tier > 3) throw new System.ArgumentException($"Tier must be between 1 and 3 (got {tier})");
        this.tier = tier;
        UpdateTexture(atlas.GetSprite($"SprinklerT{tier}"));
    }

    public new void PlaceMouseoverEffect(){
        if (hasBeenPlaced) return;
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        base.PlaceMouseoverEffect();
        Vector3Int[] coverageArea = tier switch{
            1 => GetCrossAroundPosition(currentCell).ToArray(),
            2 => GetAreaAroundPosition(currentCell, 1).ToArray(),
            3 => GetAreaAroundPosition(currentCell, 2).ToArray(),
            _ => throw new System.ArgumentException($"Invalid tier {tier}")
        };
        foreach (Vector3Int cell in coverageArea) GetComponent<Tilemap>().SetTile(cell, greenTile);
    }

    public new void Place(Vector3Int position){
        base.Place(position);
        Vector3Int[] coverageArea = tier switch{
            1 => GetCrossAroundPosition(position).ToArray(),
            2 => GetAreaAroundPosition(position, 1).ToArray(),
            3 => GetAreaAroundPosition(position, 2).ToArray(),
            _ => throw new System.ArgumentException($"Invalid tier {tier}")
        };
        foreach (Vector3Int cell in coverageArea) GetComponent<Tilemap>().SetTile(cell, null);
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        throw new NotImplementedException();
    }
}
