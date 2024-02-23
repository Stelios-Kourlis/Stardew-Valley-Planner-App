using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using static Utility.TilemapManager;
using System.Runtime.Remoting.Messaging;

public class Scarecrow : Building{

    private SpriteAtlas atlas;
    private int scarecrowIndex = 0;
    private Tile greenTile;

    public new void Start(){
        Init();
        base.Start();
        PlacePreview = PlaceMouseoverEffect;
        atlas = Resources.Load<SpriteAtlas>("Buildings/ScarecrowAtlas");
        greenTile = LoadTile("GreenTile");
        sprite = atlas.GetSprite("Scarecrows_9");
    }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 1;
    }

    private new void PlaceMouseoverEffect(){
        if (hasBeenPlaced) return;
        GetComponent<Tilemap>();
        base.PlaceMouseoverEffect();
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3Int[] coverageArea = scarecrowIndex switch{
            9 => null,
            _ => GetCircleAroundPosition(currentCell, 8).ToArray()
        };
        foreach (Vector3Int cell in coverageArea) GetComponent<Tilemap>().SetTile(cell, greenTile);
    }

    private void CycleTexture(){
        if (!hasBeenPlaced) return;
        scarecrowIndex++;
        if (scarecrowIndex > 9) scarecrowIndex = 0;
        UpdateTexture(atlas.GetSprite($"Scarecrows_{scarecrowIndex}"));
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return scarecrowIndex switch{
            9 => new Dictionary<Materials, int>(){
                {Materials.Wood, 50},
                {Materials.IridiumOre, 1},
                {Materials.Fiber, 40}
            },
            1 => new Dictionary<Materials, int>(){
                {Materials.Wood, 50},
                {Materials.Coal, 1},
                {Materials.Fiber, 40}
            },
            _ => throw new System.Exception("Rarecrows not supported (yet)")
        };
    }
}
