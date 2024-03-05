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
    private int scarecrowIndex = 9;
    private Tile greenTile;

    public new void Start(){
        name = GetType().Name;
        baseHeight = 1;
        base.Start();
        atlas = Resources.Load<SpriteAtlas>("Buildings/ScarecrowAtlas");
        greenTile = LoadTile("GreenTile");
        sprite = atlas.GetSprite("Scarecrows_9");
    }

    protected override void PlacePreview(){
        if (hasBeenPlaced) return;
        GetComponent<Tilemap>();
        base.PlacePreview();
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3Int[] coverageArea = scarecrowIndex switch{
            9 => GetCircleAroundPosition(currentCell, 16).ToArray(),
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

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return scarecrowIndex switch{
            9 => new List<MaterialInfo>{//Deluxe scarecrow
                new MaterialInfo(50, Materials.Wood),
                new MaterialInfo(1, Materials.IridiumOre),
                new MaterialInfo(40, Materials.Fiber)
            },
            0 => new List<MaterialInfo>{//Normal scarecrow
                new MaterialInfo(50, Materials.Wood),
                new MaterialInfo(1, Materials.Coal),
                new MaterialInfo(40, Materials.Fiber)
            },
            1 => new List<MaterialInfo>{//Rarecrows in order
                new MaterialInfo("Purchase at the Stardew Valley Fair for 800 Tokens"),
            },
            2 => new List<MaterialInfo>{
                new MaterialInfo("	Purchase at the Spirit's Eve festival for 5,000 Coins"),
            },
            3 => new List<MaterialInfo>{
                new MaterialInfo("Purchase at the Casino for 10,000 Qi Coins"),
            },
            4 => new List<MaterialInfo>{
                new MaterialInfo("Purchase at the Traveling Cart randomly during fall or winter for 4,000 Coins, or purchase at the Festival of Ice for 5,000 Coins"),
            },
            5 => new List<MaterialInfo>{
                new MaterialInfo("Purchase at the Flower Dance for 2,500 Coins"),
            },
            6 => new List<MaterialInfo>{
                new MaterialInfo("Purchase from the Dwarf for 2,500 Coins"),
            },
            7 => new List<MaterialInfo>{
                new MaterialInfo("Donate 20 Artifacts (not counting Minerals) to the Museum. Can be purchased from the Night Market once the first one is earned"),
            },
            8 => new List<MaterialInfo>{
                new MaterialInfo("Donate 40 items to the Museum. Can be purchased from the Night Market once the first one is earned"),
            },
            _ => throw new System.ArgumentException($"Invalid scarecrow index {scarecrowIndex}")
        };
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{scarecrowIndex}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Start();
        Place(new Vector3Int(x,y,0));
        UpdateTexture(atlas.GetSprite($"Scarecrows_{data[0]}"));
    }

    protected override void OnMouseRightClick(){
        CycleTexture();
    }
}
