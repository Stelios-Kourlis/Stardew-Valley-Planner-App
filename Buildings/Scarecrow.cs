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
using System;

public class Scarecrow : Building, IMultipleTypeBuilding<Scarecrow.Types>{

    public enum Types{
        Scarecrow,
        Rarecrow1,
        Rarecrow2,
        Rarecrow3,
        Rarecrow4,
        Rarecrow5,
        Rarecrow6,
        Rarecrow7,
        Rarecrow8,
        DeluxeScarecrow
    }
    private Tile greenTile;//todo add range interface
    private bool IsDeluxe {get; set;} = false;

    public override string TooltipMessage{get{
        if (!IsDeluxe) return "Right Click To Cycle Scarecrow Type";
        else return "";
    }}

    public MultipleTypeBuilding<Types> MultipleTypeBuildingComponent {get; private set;}

    public override void OnAwake(){
        name = GetType().Name;
        BaseHeight = 1;
        MultipleTypeBuildingComponent = new MultipleTypeBuilding<Types>(this);
        base.OnAwake();
        greenTile = LoadTile("GreenTile");
    }

    // public void SetDeluxe(){
    //     IsDeluxe = true;
    //     scarecrowIndex = 9;
    //     UpdateTexture(atlas.GetSprite($"Scarecrows_9"));
    // }

    protected override void PlacePreview(Vector3Int position){
        if (hasBeenPlaced) return;
        GetComponent<Tilemap>();
        base.PlacePreview(position);
        // Vector3Int[] coverageArea = scarecrowIndex switch{
        //     9 => GetRangeOfDeluxeScarecrow(position).ToArray(),
        //     _ => GetRangeOfScarecrow(position).ToArray()
        // };
        // foreach (Vector3Int cell in coverageArea) GetComponent<Tilemap>().SetTile(cell, greenTile);
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return MultipleTypeBuildingComponent.Type switch{
            Types.DeluxeScarecrow => new List<MaterialInfo>{//Deluxe scarecrow
                new MaterialInfo(50, Materials.Wood),
                new MaterialInfo(1, Materials.IridiumOre),
                new MaterialInfo(40, Materials.Fiber)
            },
            Types.Scarecrow => new List<MaterialInfo>{//Normal scarecrow
                new MaterialInfo(50, Materials.Wood),
                new MaterialInfo(1, Materials.Coal),
                new MaterialInfo(40, Materials.Fiber)
            },
            Types.Rarecrow1 => new List<MaterialInfo>{//Rarecrows in order
                new MaterialInfo("Purchase at the Stardew Valley Fair for 800 Tokens"),
            },
            Types.Rarecrow2 => new List<MaterialInfo>{
                new MaterialInfo("	Purchase at the Spirit's Eve festival for 5,000 Coins"),
            },
            Types.Rarecrow3 => new List<MaterialInfo>{
                new MaterialInfo("Purchase at the Casino for 10,000 Qi Coins"),
            },
            Types.Rarecrow4 => new List<MaterialInfo>{
                new MaterialInfo("Purchase at the Traveling Cart randomly during fall or winter for 4,000 Coins, or purchase at the Festival of Ice for 5,000 Coins"),
            },
            Types.Rarecrow5 => new List<MaterialInfo>{
                new MaterialInfo("Purchase at the Flower Dance for 2,500 Coins"),
            },
            Types.Rarecrow6 => new List<MaterialInfo>{
                new MaterialInfo("Purchase from the Dwarf for 2,500 Coins"),
            },
            Types.Rarecrow7 => new List<MaterialInfo>{
                new MaterialInfo("Donate 20 Artifacts (not counting Minerals) to the Museum. Can be purchased from the Night Market once the first one is earned"),
            },
            Types.Rarecrow8 => new List<MaterialInfo>{
                new MaterialInfo("Donate 40 items to the Museum. Can be purchased from the Night Market once the first one is earned"),
            },
            _ => throw new System.ArgumentException($"Invalid scarecrow type {MultipleTypeBuildingComponent.Type}")
        };
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{MultipleTypeBuildingComponent.Type}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
        MultipleTypeBuildingComponent.SetType((Types) int.Parse(data[0]));
    }

    protected override void OnMouseRightClick(){
        MultipleTypeBuildingComponent.CycleType();
    }

    public GameObject[] CreateButtonsForAllTypes(){
        return MultipleTypeBuildingComponent.CreateButtonsForAllTypes();
    }
}
