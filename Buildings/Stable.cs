using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stable : Building {

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 2;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.PAINT
        };
    }

    public new void Start(){
        Init();
        base.Start();
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return new Dictionary<Materials, int>(){
            {Materials.Coins, 10_000},
            {Materials.Hardwood, 100},
            {Materials.IronBar, 5},
        };
    }
}
