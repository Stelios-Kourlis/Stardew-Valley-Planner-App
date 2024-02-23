using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlimeHutch : Building {

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 6;
        insideAreaTexture = Resources.Load("BuildingInsides/Barn1") as Texture2D;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.ENTER
        };
    }

    public new void Start(){
        Init();
        base.Start();
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return new Dictionary<Materials, int>(){
            {Materials.Coins, 10_000},
            {Materials.Stone, 500},
            {Materials.RefinedQuartz, 10},
            {Materials.IridiumBar, 1},
        };
    }
}
