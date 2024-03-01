using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlimeHutch : Building {

    public new void Start(){
        name = GetType().Name;
        baseHeight = 6;
        insideAreaTexture = Resources.Load("BuildingInsides/Barn1") as Texture2D;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.ENTER
        };
        base.Start();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo>(){
            new MaterialInfo(10000, Materials.Coins),
            new MaterialInfo(500, Materials.Stone),
            new MaterialInfo(10, Materials.RefinedQuartz),
            new MaterialInfo(1, Materials.IridiumBar)
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Start();
        Place(new Vector3Int(x,y,0));
    }
}
