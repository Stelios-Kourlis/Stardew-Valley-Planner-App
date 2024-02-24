using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class JunimoHut : Building {
    public new void Start(){
        name = GetType().Name;
        baseHeight = 2;
        base.Start();
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return new Dictionary<Materials, int>(){
            {Materials.Coins, 20_000},
            {Materials.Stone, 200},
            {Materials.Starfruit, 9},
            {Materials.Fiber, 100}
        };
    }

    protected override void PlacePreview(){//todo add range
        base.PlacePreview();
    }
}