using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Mill : Building {
        
    public override string TooltipMessage => "Right Click For More Options";
    
    public override void OnAwake(){
        name = GetType().Name;
        baseHeight = 2;
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo>(){
            new MaterialInfo(2500, Materials.Coins),
            new MaterialInfo(50, Materials.Stone),
            new MaterialInfo(150, Materials.Wood),
            new MaterialInfo(4, Materials.Cloth)
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
    }
}

