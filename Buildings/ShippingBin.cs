using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShippingBin : Building {

    protected override void Init() {
        name = GetType().Name;
        baseHeight = 1;
    }

    public new void Start(){
        Init();
        base.Start();
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return new Dictionary<Materials, int>(){
            {Materials.Coins, 250},
            {Materials.Wood, 150}
        };
    }
}