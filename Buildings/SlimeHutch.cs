using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlimeHutch : Building {
    public override string TooltipMessage => "Right Click For More Options";

    public override void OnAwake(){
        name = GetType().Name;
        BaseHeight = 4;
        // insideAreaTexture = Resources.Load("BuildingInsides/Barn1") as Texture2D;
        BuildingInteractions = new ButtonTypes[]{
            ButtonTypes.ENTER
        };
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo>(){
            new(10000, Materials.Coins),
            new(500, Materials.Stone),
            new(10, Materials.RefinedQuartz),
            new(1, Materials.IridiumBar)
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
    }
}
