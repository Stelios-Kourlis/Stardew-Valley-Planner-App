using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShippingBin : Building {

    public override void OnAwake() {
        BuildingName = "Shipping Bin";
        BaseHeight = 1;
        base.OnAwake();
    }

    public override List<MaterialCostEntry> GetMaterialsNeeded() {
        return new List<MaterialCostEntry>(){
            new(250, Materials.Coins),
            new(150, Materials.Wood)
        };
    }
}