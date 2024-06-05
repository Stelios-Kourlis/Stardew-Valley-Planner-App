using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Well : Building {

    public override void OnAwake() {
        BuildingName = "Well";
        BaseHeight = 3;
        base.OnAwake();
    }

    public override List<MaterialInfo> GetMaterialsNeeded() {
        return new List<MaterialInfo>(){
        new(1000, Materials.Coins),
        new(75, Materials.Stone),
        };
    }
}